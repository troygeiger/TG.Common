using System;
using System.Reflection;

namespace TG.Common
{
    /// <summary>
    /// A helper for collecting assembly information such as Product, Version...
    /// </summary>
    public static class AssemblyInfo
    {
        private static Assembly _referenceAssembly = null;

        /// <summary>
        /// The <see cref="Assembly"/> to collect information from.
        /// </summary>
        public static Assembly ReferenceAssembly
        {
            get
            {
                if (_referenceAssembly == null)
                {
                    _referenceAssembly = Assembly.GetEntryAssembly();
                }
                return _referenceAssembly;
            }
            set
            {
                _referenceAssembly = value;
            }
        }

        /// <summary>
        /// Gets the value from the Company attribute.
        /// </summary>
        public static string Company => GetEntryAssemblyAttribute<AssemblyCompanyAttribute>(a => a.Company);

        /// <summary>
        /// Gets the value from the Product attribute.
        /// </summary>
        public static string Product => GetEntryAssemblyAttribute<AssemblyProductAttribute>(a => a.Product);

        /// <summary>
        /// Gets the value from the Copyright attribute.
        /// </summary>
        public static string Copyright => GetEntryAssemblyAttribute<AssemblyCopyrightAttribute>(a => a.Copyright);

        /// <summary>
        /// Gets the value from the Trademark attribute.
        /// </summary>
        public static string Trademark => GetEntryAssemblyAttribute<AssemblyTrademarkAttribute>(a => a.Trademark);

        /// <summary>
        /// Gets the value from the Title attribute.
        /// </summary>
        public static string Title => GetEntryAssemblyAttribute<AssemblyTitleAttribute>(a => a.Title);

        /// <summary>
        /// Gets the value from the Description attribute.
        /// </summary>
        public static string Description => GetEntryAssemblyAttribute<AssemblyDescriptionAttribute>(a => a.Description);

        /// <summary>
        /// Gets the value from the Configuration attribute.
        /// </summary>
        public static string Configuration => GetEntryAssemblyAttribute<AssemblyConfigurationAttribute>(a => a.Configuration);

        /// <summary>
        /// Gets the value from the Version attribute.
        /// </summary>
        public static Version Version => ReferenceAssembly.GetName().Version;

        /// <summary>
        /// Gets the value from the version string from the Version attribute.
        /// </summary>
        public static string VersionString => Version.ToString();

        /// <summary>
        /// Gets the value from the FileVersion attribute.
        /// </summary>
        public static string FileVersion => GetEntryAssemblyAttribute<AssemblyFileVersionAttribute>(a => a.Version);

        /// <summary>
        /// Gets the value from the InformationVersion attribute.
        /// </summary>
        public static string InformationVersion => GetEntryAssemblyAttribute<AssemblyInformationalVersionAttribute>(a => a.InformationalVersion);

        private static string GetEntryAssemblyAttribute<T>(Func<T, string> value) where T : Attribute
        {
            T attribute = (T)Attribute.GetCustomAttribute(ReferenceAssembly, typeof(T));
            if (attribute == null) return string.Empty;
            return value.Invoke(attribute);
        }

#if NET20

        internal delegate TOut Func<TIn, TOut>(TIn attribute);

#endif
    }
}