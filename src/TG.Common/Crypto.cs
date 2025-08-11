using System;
using System.Security.Cryptography;
using System.Text;

namespace TG.Common
{
    /// <summary>
    /// Provides symmetric encryption. New encryptions use AES-CBC with a random IV prepended to the ciphertext.
    /// Decryption supports AES format and falls back to legacy Rijndael with a fixed IV for backward compatibility.
    /// </summary>
    public class Crypto : IDisposable
    {
        private byte[] cryptKey;
        // Legacy fixed IV used by older Rijndael implementation; retained for backward compatibility during fallback.
        private static readonly byte[] LegacyIv = new byte[] { 68, 65, 43, 114, 98, 118, 120, 103, 101, 79, 102, 107, 100, 111, 51, 33 };

        private readonly Aes aes;

        /// <summary>
        /// Creates an instance of <see cref="Crypto"/>.
        /// </summary>
        /// <param name="key">The key, no larger than 32 bytes, to use during encryption and decryption.</param>
        public Crypto(byte[] key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (key.Length > 32)
                throw new CryptographicException("Key size too large.");

            // Pad/normalize to 256-bit key to preserve previous behavior.
            cryptKey = new byte[32];
            for (int i = 0; i < key.Length; i++)
                cryptKey[i] = key[i];

            aes = Aes.Create();
            aes.Key = cryptKey;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
        }

        /// <summary>
        /// Creates an instance of <see cref="Crypto"/>.
        /// </summary>
        /// <param name="key">The key to use during encryption and decryption.</param>
        public Crypto(string key) : this(Encoding.UTF8.GetBytes(key)) { }

        /// <summary>
        /// Encrypts a byte array using AES-CBC. The returned payload is IV || CIPHERTEXT.
        /// </summary>
        /// <param name="bytes">A byte array to encrypt.</param>
        /// <returns>Encrypted byte array, or null if the key is invalid.</returns>
        public byte[] Encrypt(byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));
            if (cryptKey == null)
                return null;
            if (cryptKey.Length < 32)
                return null;

            byte[] iv = new byte[aes.BlockSize / 8]; // 16 bytes
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(iv);
            }

            using (var enc = aes.CreateEncryptor(cryptKey, iv))
            using (var ms = new System.IO.MemoryStream())
            {
                // Reserve space for IV + ciphertext
                ms.Write(iv, 0, iv.Length);
                using (var cs = new CryptoStream(ms, enc, CryptoStreamMode.Write))
                {
                    cs.Write(bytes, 0, bytes.Length);
                    cs.FlushFinalBlock();
                }
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Encrypts text to a byte array.
        /// </summary>
        public byte[] Encrypt(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            return Encrypt(Encoding.Unicode.GetBytes(text));
        }

        /// <summary>
        /// Encrypts text to a base64 string.
        /// </summary>
        public string EncryptBase64(string text)
        {
            return Convert.ToBase64String(Encrypt(text));
        }

        /// <summary>
        /// Encrypts a byte array to a base64 string.
        /// </summary>
        public string EncryptBase64(byte[] bytes)
        {
            return Convert.ToBase64String(Encrypt(bytes));
        }

        /// <summary>
        /// Decrypts a byte array. Attempts AES (IV-prefixed) first, then falls back to legacy Rijndael (fixed IV).
        /// </summary>
        /// <param name="bytes">A byte array to decrypt.</param>
        /// <returns>Unencrypted byte array.</returns>
        public byte[] Decrypt(byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));
            if (cryptKey == null)
                return null;
            if (cryptKey.Length < 32)
                return null;

            // Try current AES format: first 16 bytes are IV
            try
            {
                if (bytes.Length > aes.BlockSize / 8)
                {
                    int ivLen = aes.BlockSize / 8;
                    var iv = new byte[ivLen];
                    Buffer.BlockCopy(bytes, 0, iv, 0, ivLen);
                    using (var dec = aes.CreateDecryptor(cryptKey, iv))
                    using (var ms = new System.IO.MemoryStream())
                    using (var cs = new CryptoStream(ms, dec, CryptoStreamMode.Write))
                    {
                        cs.Write(bytes, ivLen, bytes.Length - ivLen);
                        cs.FlushFinalBlock();
                        return ms.ToArray();
                    }
                }
            }
            catch (CryptographicException)
            {
                // Fallback handled below
            }

            // Fallback: legacy Rijndael with fixed IV and no IV prefix in payload
            try
            {
                using (var legacy = Rijndael.Create())
                {
                    legacy.Key = cryptKey;
                    legacy.Mode = CipherMode.CBC;
                    legacy.Padding = PaddingMode.PKCS7;
                    using (var dec = legacy.CreateDecryptor(cryptKey, LegacyIv))
                    using (var ms = new System.IO.MemoryStream())
                    using (var cs = new CryptoStream(ms, dec, CryptoStreamMode.Write))
                    {
                        cs.Write(bytes, 0, bytes.Length);
                        cs.FlushFinalBlock();
                        return ms.ToArray();
                    }
                }
            }
            catch (CryptographicException)
            {
                // Re-throw a consistent exception for invalid payloads
                throw;
            }
        }

        /// <summary>
        /// Decrypts byte array to a Unicode string.
        /// </summary>
        public string DecryptToString(byte[] bytes)
        {
            return Encoding.Unicode.GetString(Decrypt(bytes));
        }

        /// <summary>
        /// Decrypts a base64 string to an unencrypted Unicode string.
        /// </summary>
        public string DecryptBase64(string base64)
        {
            if (base64 == null) throw new ArgumentNullException(nameof(base64));
            return DecryptToString(Convert.FromBase64String(base64));
        }

        /// <summary>
        /// Decrypts a base64 string to an unencrypted byte array.
        /// </summary>
        public byte[] DecryptBase64ToByte(string base64)
        {
            if (base64 == null) throw new ArgumentNullException(nameof(base64));
            return Decrypt(Convert.FromBase64String(base64));
        }

        /// <summary>
        /// Disposes the Crypto class.
        /// </summary>
        public void Dispose()
        {
            aes?.Dispose();
            // Clear sensitive data
            if (cryptKey != null)
                Array.Clear(cryptKey, 0, cryptKey.Length);
            cryptKey = null;
        }
    }
}
