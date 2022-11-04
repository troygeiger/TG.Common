
namespace System.Reflection
{
    public static class AssemblyExtensions
    {
        public static string GetCompany(this Assembly assembly)
            => GetAssemblyAttributeValue<AssemblyCompanyAttribute, string>(assembly, a => a.Company);

        public static string GetProduct(this Assembly assembly)
            => GetAssemblyAttributeValue<AssemblyProductAttribute, string>(assembly, a => a.Product);

        public static string GetCopyright(this Assembly assembly)
            => GetAssemblyAttributeValue<AssemblyCopyrightAttribute, string>(assembly, a => a.Copyright);

        public static string GetTrademark(this Assembly assembly)
            => GetAssemblyAttributeValue<AssemblyTrademarkAttribute, string>(assembly, a => a.Trademark);

        public static string GetTitle(this Assembly assembly)
            => GetAssemblyAttributeValue<AssemblyTitleAttribute, string>(assembly, a => a.Title);

        public static string GetDescription(this Assembly assembly)
            => GetAssemblyAttributeValue<AssemblyDescriptionAttribute, string>(assembly, a => a.Description);

        public static Version GetVersion(this Assembly assembly)
            => GetAssemblyAttributeValue<AssemblyVersionAttribute, Version>(assembly, a => a.Version);

        public static Version GetFileVersion(this Assembly assembly)
            => GetAssemblyAttributeValue<AssemblyFileVersionAttribute, Version>(assembly, a => a.Version);

        public static string GetInformationalVersion(this Assembly assembly)
            => GetAssemblyAttributeValue<AssemblyInformationalVersionAttribute, Version>(assembly, a => a.InformationalVersion);

        public static string GetConfiguration(this Assembly assembly)
            => GetAssemblyAttributeValue<AssemblyConfigurationAttribute, string>(assembly, a => a.Configuration);

        private static TValue GetAssemblyAttributeValue<TAttribute, TValue>(Assembly assembly, Func<TAttribute, TValue> value) where T : Attribute
        {
            T attribute = (T)Attribute.GetCustomAttribute(assembly, typeof(TAttribute));
            return value.Invoke(attribute);
        }

    }
}