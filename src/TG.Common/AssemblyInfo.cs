using System;
using System.Reflection;

namespace TG.Common
{

    [Obsolete("Use AssemblyInformation or Assembly.GetAssemblyInformation()")]
    static public class AssemblyInfo
    {
        static Assembly _callingAssembly = null;

        private static Assembly CallingAssembly
        {
            get
            {
                if (_callingAssembly == null)
                {
                    System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
                    System.Reflection.MethodBase meth = null;
                    for (int i = 0; i < trace.FrameCount; i++)
                    {
                        meth = trace.GetFrame(i).GetMethod();
                        if (meth.DeclaringType.Namespace != "TG.Common")//.Name != "WriteToLog" && meth.Name != "WriteExceptionToLog")
                            break;
                    }
                    _callingAssembly = meth.DeclaringType.Assembly;
                }
                return _callingAssembly;
            }
        }

        public static string Company { get { return GetEntryAssemblyAttribute<AssemblyCompanyAttribute>(a => a.Company); } }
        public static string Product { get { return GetEntryAssemblyAttribute<AssemblyProductAttribute>(a => a.Product); } }
        public static string Copyright { get { return GetEntryAssemblyAttribute<AssemblyCopyrightAttribute>(a => a.Copyright); } }
        public static string Trademark { get { return GetEntryAssemblyAttribute<AssemblyTrademarkAttribute>(a => a.Trademark); } }
        public static string Title { get { return GetEntryAssemblyAttribute<AssemblyTitleAttribute>(a => a.Title); } }
        public static string Description { get { return GetEntryAssemblyAttribute<AssemblyDescriptionAttribute>(a => a.Description); } }
        public static string Configuration { get { return GetEntryAssemblyAttribute<AssemblyDescriptionAttribute>(a => a.Description); } }
        public static string FileVersion { get { return GetEntryAssemblyAttribute<AssemblyFileVersionAttribute>(a => a.Version); } }

        public static Version Version { get { return CallingAssembly.GetName().Version; } }
        public static string VersionFull { get { return Version.ToString(); } }
        public static string VersionMajor { get { return Version.Major.ToString(); } }
        public static string VersionMinor { get { return Version.Minor.ToString(); } }
        public static string VersionBuild { get { return Version.Build.ToString(); } }
        public static string VersionRevision { get { return Version.Revision.ToString(); } }
        
        private static string GetEntryAssemblyAttribute<T>(Func<T, string> value) where T : Attribute
        {
            T attribute = (T)Attribute.GetCustomAttribute(CallingAssembly, typeof(T));
            return value.Invoke(attribute);
        }

    }


}
