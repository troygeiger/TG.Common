using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace TG.Common
{
    /// <summary>
    /// Helper for getting and creating user profile AppData folders.
    /// </summary>
    public static class AppData
    {
        /// <summary>
        /// Determines how the AppDataPath method will generate the path.
        /// </summary>
        public enum AppDataSchemes
        {
            /// <summary>
            /// Generates %AppData%\AssemblyInfo.Company\AssemblyInfo.Title
            /// </summary>
            CompanyTitle,
            /// <summary>
            /// Generates %AppData%\AssemblyInfo.Company\AssemblyInfo.Product
            /// </summary>
            CompanyProduct,
            /// <summary>
            /// Generates %AppData%\AssemblyInfo.Company
            /// </summary>
            Company,
            /// <summary>
            /// Generates %AppData%\AssemblyInfo.Title
            /// </summary>
            Title,
            /// <summary>
            /// Generates %AppData%\AssemblyInfo.Product
            /// </summary>
            Product
        }

        /// <summary>
        /// The default scheme used for <see cref="AppDataPath"/>.
        /// </summary>
        /// <value></value>
        public static AppDataSchemes DefaultScheme { get; set; } = AppDataSchemes.CompanyTitle;

        /// <summary>
        /// Gets the %AppData%\[Assembly.Company]\[Assembly.Title] path. If the path does not exist, it will be created before returning the path.
        /// </summary>
        public static string AppDataPath
        {
            get
            {
                return GetAppDataPath(DefaultScheme);
            }
        }

        /// <summary>
        /// Gets the %AppData%\... path based on the provided scheme. If the path does not exist, it will be created before returning the path.
        /// </summary>
        /// <param name="scheme">Specifies how the path should be generated.</param>
        /// <returns></returns>
        public static string GetAppDataPath(AppDataSchemes scheme)
        {
            if ((scheme == AppDataSchemes.CompanyTitle || scheme == AppDataSchemes.Company) && string.IsNullOrEmpty(AssemblyInfo.Company))
                throw new Exception("Assembly Company is not set.");
            if ((scheme == AppDataSchemes.CompanyTitle || scheme == AppDataSchemes.Title) && string.IsNullOrEmpty(AssemblyInfo.Title))
                throw new Exception("Assembly Title is not set.");

            string path = string.Empty;
            switch (scheme)
            {
                case AppDataSchemes.CompanyTitle:
                    path = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            GetPathSafeString(AssemblyInfo.Company)), GetPathSafeString(AssemblyInfo.Title));
                    break;
                case AppDataSchemes.CompanyProduct:
                    path = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            GetPathSafeString(AssemblyInfo.Company)), GetPathSafeString(AssemblyInfo.Product));
                    break;
                case AppDataSchemes.Company:
                    path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            GetPathSafeString(AssemblyInfo.Company));
                    break;
                case AppDataSchemes.Title:
                    path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            GetPathSafeString(AssemblyInfo.Title));
                    break;
                case AppDataSchemes.Product:
                    path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            GetPathSafeString(AssemblyInfo.Product));
                    break;
                default:
                    break;
            }

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

    private static string GetPathSafeString(string segment)
        {
            foreach (var item in Path.GetInvalidFileNameChars())
                segment = segment.Replace(new string(new char[] { item }), "");
            return segment;
        }
    }
}
