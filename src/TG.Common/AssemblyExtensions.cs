namespace System.Reflection
{
    /// <summary>
    /// Extension methods for <see cref="Assembly"/>
    /// </summary>
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Gets the value from the Configuration attribute property.
        /// </summary>
        /// <param name="assembly">The assembly to collect information from.</param>
        /// <returns><see cref="string"/></returns>
        public static string GetConfiguration(this Assembly assembly)
            => GetAssemblyAttributeValue<AssemblyConfigurationAttribute, string>(assembly, a => a.Configuration);

        /// <summary>
        /// Gets the value from the Company attribute property.
        /// </summary>
        /// <param name="assembly">The assembly to collect information from.</param>
        /// <returns><see cref="string"/></returns>
        public static string GetCompany(this Assembly assembly)
            => GetAssemblyAttributeValue<AssemblyCompanyAttribute, string>(assembly, a => a.Company);

        /// <summary>
        /// Gets the value from the Product attribute property.
        /// </summary>
        /// <param name="assembly">The assembly to collect information from.</param>
        /// <returns><see cref="string"/></returns>
        public static string GetProduct(this Assembly assembly)
            => GetAssemblyAttributeValue<AssemblyProductAttribute, string>(assembly, a => a.Product);

        /// <summary>
        /// Gets the value from the Copyright attribute property.
        /// </summary>
        /// <param name="assembly">The assembly to collect information from.</param>
        /// <returns><see cref="string"/></returns>
        public static string GetCopyright(this Assembly assembly)
            => GetAssemblyAttributeValue<AssemblyCopyrightAttribute, string>(assembly, a => a.Copyright);

        /// <summary>
        /// Gets the value from the Trademark attribute property.
        /// </summary>
        /// <param name="assembly">The assembly to collect information from.</param>
        /// <returns><see cref="string"/></returns>
        public static string GetTrademark(this Assembly assembly)
            => GetAssemblyAttributeValue<AssemblyTrademarkAttribute, string>(assembly, a => a.Trademark);

        /// <summary>
        /// Gets the value from the Title attribute property.
        /// </summary>
        /// <param name="assembly">The assembly to collect information from.</param>
        /// <returns><see cref="string"/></returns>
        public static string GetTitle(this Assembly assembly)
            => GetAssemblyAttributeValue<AssemblyTitleAttribute, string>(assembly, a => a.Title);

        /// <summary>
        /// Gets the value from the Description attribute property.
        /// </summary>
        /// <param name="assembly">The assembly to collect information from.</param>
        /// <returns><see cref="string"/></returns>
        public static string GetDescription(this Assembly assembly)
            => GetAssemblyAttributeValue<AssemblyDescriptionAttribute, string>(assembly, a => a.Description);

        /// <summary>
        /// Gets the value from the Version attribute property.
        /// </summary>
        /// <param name="assembly">The assembly to collect information from.</param>
        /// <returns><see cref="Version"/></returns>
        public static Version GetVersion(this Assembly assembly)
            => assembly.GetName().Version;

        /// <summary>
        /// Gets the value from the FileVersion attribute property.
        /// </summary>
        /// <param name="assembly">The assembly to collect information from.</param>
        /// <returns><see cref="string"/></returns>
        public static string GetFileVersion(this Assembly assembly)
            => GetAssemblyAttributeValue<AssemblyFileVersionAttribute, string>(assembly, a => a.Version);

        /// <summary>
        /// Gets the value from the InformationalVersion attribute property.
        /// </summary>
        /// <param name="assembly">The assembly to collect information from.</param>
        /// <returns><see cref="string"/></returns>
        public static string GetInformationalVersion(this Assembly assembly)
            => GetAssemblyAttributeValue<AssemblyInformationalVersionAttribute, string>(assembly, a => a.InformationalVersion);

        
        private static TValue GetAssemblyAttributeValue<TAttribute, TValue>(Assembly assembly, Func<TAttribute, TValue> value) where TAttribute : Attribute
        {
            TAttribute attribute = (TAttribute)Attribute.GetCustomAttribute(assembly, typeof(TAttribute));
            return value.Invoke(attribute);
        }
    }
}