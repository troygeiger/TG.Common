using NUnit.Framework;
using TG.Common;

namespace UnitTests
{
    public class TestCrypto
    {
        Crypto? crypto = null;

        [SetUp]
        public void Setup()
        {
            crypto = new(nameof(TestCrypto));
        }

        [TearDown]
        public void TearDown()
        {
            crypto?.Dispose();
        }

        [Test]
        [TestCase("Hello, world")]
        public void TestEncryptionRoundTrip(string stringToEncrypt)
        {
            var bytes = crypto!.Encrypt(stringToEncrypt);

            Assert.IsNotNull(bytes);
            
            Assert.Greater(bytes.Length, 0);

            string result = crypto.DecryptToString(bytes);

            Assert.AreEqual(stringToEncrypt, result);
        }

        [Test]
        [TestCase("Hello, world")]
        public void TestEncryptionRoundTripBase64(string stringToEncrypt)
        {
            string encrypted = crypto!.EncryptBase64(stringToEncrypt);

            Assert.IsNotEmpty(encrypted);

            string unencrypted = crypto.DecryptBase64(encrypted);

            Assert.AreEqual(stringToEncrypt, unencrypted);
        }

    }
}