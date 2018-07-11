using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TG.Common.IO
{
    /// <summary>
    /// Provides various tools for working with IO.
    /// </summary>
    public static class IoTools
    {
        /// <summary>
        /// This ensures that the provided path does not contain any invalid characters. If so, they are replaced with underscores.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetSafePathName(string path)
        {
            string dir = Path.GetDirectoryName(path);
            string filename = Path.GetFileName(path);
            StringBuilder clean = new StringBuilder();
            char[] invPath = Path.GetInvalidPathChars();
            string tmp;

            foreach (string item in dir.Split(Path.PathSeparator))
            {
                tmp = item;
                foreach (char invalid in invPath)
                {
                    tmp = tmp.Replace(invalid, '_');
                }
                clean.Append($"{tmp}{Path.PathSeparator}");
            }

            foreach (char item in Path.GetInvalidFileNameChars())
            {
                filename = filename.Replace(item, '_');
            }
            return Path.Combine(clean.ToString(), filename);
        }
    }
}
