using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;


namespace TG.Common
{
    public static class LogManager
    {
        static bool initialized;
        static string logPath;
        static string logFolder;
        static DateTime logDate;
        static object LogLock = new object();

        public static void InitializeLog()
        {
            logDate = DateTime.Now.Date;

            try
            {
                logFolder = Path.Combine(AppData.AppDataPath, "Logging");
                if (!Directory.Exists(logFolder))
                    Directory.CreateDirectory(logFolder);
                logPath = Path.Combine(logFolder, $"{logDate.ToShortDateString().Replace("/", "-")}.txt");
                initialized = true;
            }
            catch (Exception ex)
            {
                initialized = false;
            }

        }
        
        public static string LogPath
        {
            get { return logPath; }
            set
            {
                logPath = value;
                initialized = !string.IsNullOrEmpty(logPath) && Directory.Exists(Path.GetDirectoryName(logPath));
            }
        }

        public static bool IsInitialized { get { return initialized; } }
        
        public static bool IsDebugging { get; set; }

        //[DebuggerStepThrough]
        public static void WriteToLog(string Location, string Message, string ExceptionMessage)
        {
            lock (LogLock)
            {
                if (!initialized)
                {
                    InitializeLog();
                    if (!initialized) return;
                }
                

                if (string.IsNullOrEmpty(Location))
                {
                    System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
                    System.Reflection.MethodBase meth = null;
                    for (int i = 0; i < trace.FrameCount; i++)
                    {
                        meth = trace.GetFrame(i).GetMethod();
                        if (meth.DeclaringType.Namespace != "TG.Common")//.Name != "WriteToLog" && meth.Name != "WriteExceptionToLog")
                            break;
                    }

                    Location = meth.DeclaringType.FullName + "." + meth.Name;

                }
                Message = string.Format("[{0}] <{1}>: {2}", new object[] { DateTime.Now.ToString(), Location, Message });
                Console.ForegroundColor = ConsoleColor.White;
                bool isException = !string.IsNullOrEmpty(ExceptionMessage);
                if (isException) Message += "\r\n>>" + ExceptionMessage;
                //#if DEBUG
                if (IsDebugging)
                {
                    if (isException)
                        Console.ForegroundColor = ConsoleColor.Red;

                    Console.WriteLine(Message);
                }

                //#endif


                try
                {
                    System.IO.File.AppendAllText(logPath, Message + "\r\n");
                }
                catch (Exception)
                {


                }
            }
        }

        public static void WriteToLog(string Location, string Message)
        {
            WriteToLog(Location, Message, null);
        }

        public static void WriteToLog(string Message)
        {
            WriteToLog(null, Message, null);
        }

        public static void WriteExceptionToLog(string message, Exception ex)
        {
            System.Text.StringBuilder exmessage = new System.Text.StringBuilder();
            exmessage.AppendLine(ex.Message);
            exmessage.AppendLine(ex.StackTrace);
            if (ex.InnerException != null)
            {
                exmessage.AppendLine("=========Inner Exception==============");
                exmessage.AppendLine(ex.InnerException.Message);
                exmessage.AppendLine(ex.InnerException.StackTrace);
            }
            WriteToLog(null, message, exmessage.ToString());
        }

        public static void WriteExceptionToLog(string exceptionMessage)
        {
            WriteToLog(null, "Exception", exceptionMessage);
        }

        public static void WriteExceptionToLog(Exception ex)
        {
            WriteExceptionToLog("Exception", ex);
        }

        /// <summary>
        /// Deletes all log files except for the most recent days specified by the keepDays parameter.
        /// </summary>
        /// <param name="keepDays">The number of days to keep.</param>
        public static void PurgeLogs(int keepDays = 30)
        {
            try
            {
                List<string> files = new List<string>(Directory.GetFiles(logFolder, "*.txt"));
                files.Sort((x, y) => {
                    DateTime xd, yd;
                    DateTime.TryParse(Path.GetFileNameWithoutExtension(x), out xd);
                    DateTime.TryParse(Path.GetFileNameWithoutExtension(y), out yd);
                    return -DateTime.Compare(xd, yd);
                });
                for (int i = keepDays; i < files.Count; i++)
                    File.Delete(files[i]);
            }
            catch (Exception)
            {

            }
        }

    }
}
