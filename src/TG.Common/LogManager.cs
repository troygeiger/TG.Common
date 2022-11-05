using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;


namespace TG.Common
{
    /// <summary>
    /// A basic logger for writing to log files.
    /// </summary>
    public static class LogManager
    {
        static bool initialized;
        static string logPath;
        static string logFolder;
        static DateTime logDate;
        static object LogLock = new object();

        /// <summary>
        /// Initializes the log at the default AppData.AppDataPath with a
        /// filename based on the localized ToShortDateString.
        /// </summary>
        public static void InitializeDefaultLog()
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
            catch (Exception)
            {
                initialized = false;
            }
        }

        /// <summary>
        /// Initializes the log at the default AppData.AppDataPath with a
        /// filename based on the localized ToShortDateString.
        /// </summary>
        [Obsolete("Use InitializeDefaultLog instead.")]
        public static void InitializeLog()
        {
            InitializeDefaultLog();
        }

        /// <summary>
        /// Gets or sets the log file path in which the logs should be written
        /// to; including the file name.
        /// </summary>
        public static string LogPath
        {
            get { return logPath; }
            set
            {
                logPath = value;
                initialized = !string.IsNullOrEmpty(logPath) && Directory.Exists(Path.GetDirectoryName(logPath));
            }
        }

        /// <summary>
        /// Indicates if <see cref="LogManager"/> is initialized, path has been
        /// verified and is ready to write logs.
        /// </summary>
        public static bool IsInitialized { get { return initialized; } }

        /// <summary>
        /// Gets or sets whether to write the log to Console.
        /// </summary>
        public static bool IsDebugging { get; set; }

        /// <summary>
        /// Writes an entry to the log file.
        /// </summary>
        /// <param name="location">Specify the location in the code in with the
        /// log entry references.</param>
        /// <param name="message">A message to include in the log.</param>
        /// <param name="exceptionMessage">Include an additional exception
        /// message below the message.</param>
        public static void WriteToLog(string location, string message, string exceptionMessage = null)
        {
            lock (LogLock)
            {
                if (!initialized)
                {
                    InitializeDefaultLog();
                    if (!initialized) return;
                }


                if (string.IsNullOrEmpty(location))
                {
                    System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
                    System.Reflection.MethodBase meth = null;
                    for (int i = 0; i < trace.FrameCount; i++)
                    {
                        meth = trace.GetFrame(i).GetMethod();
                        if (meth.DeclaringType.Namespace != "TG.Common")//.Name != "WriteToLog" && meth.Name != "WriteExceptionToLog")
                            break;
                    }

                    location = meth.DeclaringType.FullName + "." + meth.Name;

                }
                message = string.Format("[{0}] <{1}>: {2}", new object[] { DateTime.Now.ToString(), location, message });
                Console.ForegroundColor = ConsoleColor.White;
                bool isException = !string.IsNullOrEmpty(exceptionMessage);
                if (isException) message += "\r\n>>" + exceptionMessage;
                //#if DEBUG
                if (IsDebugging)
                {
                    if (isException)
                        Console.ForegroundColor = ConsoleColor.Red;

                    Console.WriteLine(message);
                }

                //#endif


                try
                {
                    System.IO.File.AppendAllText(logPath, message + "\r\n");
                }
                catch (Exception)
                {


                }
            }
        }

        /// <summary>
        /// Writes an entry to the log file.
        /// </summary>
        /// <param name="location">Specify the location in the code in with the
        /// log entry references.</param>
        /// <param name="message">A message to include in the log.</param>
        public static void WriteToLog(string location, string message)
        {
            WriteToLog(location, message, null);
        }

        /// <summary>
        /// Writes an entry to the log file.
        /// </summary>
        /// <param name="message">A message to include in the log.</param>
        public static void WriteToLog(string message)
        {
            WriteToLog(null, message, null);
        }

        /// <summary>
        /// Writes an exception to the log file.
        /// </summary>
        /// <param name="message">A message to include in the log.</param>
        /// <param name="ex">The <see cref="Exception"/> to log.</param>
        public static void WriteExceptionToLog(string message, Exception ex)
        {
            System.Text.StringBuilder exMessage = new System.Text.StringBuilder();
            exMessage.AppendLine(ex.Message);
            exMessage.AppendLine(ex.StackTrace);
            if (ex.InnerException != null)
            {
                exMessage.AppendLine("=========Inner Exception==============");
                exMessage.AppendLine(ex.InnerException.Message);
                exMessage.AppendLine(ex.InnerException.StackTrace);
            }
            WriteToLog(null, message, exMessage.ToString());
        }

        /// <summary>
        /// Writes an exception to the log file.
        /// </summary>
        /// <param name="exceptionMessage">A message to include in the log.</param>
        public static void WriteExceptionToLog(string exceptionMessage)
        {
            WriteToLog(null, "Exception", exceptionMessage);
        }

        /// <summary>
        /// Writes an exception to the log file.
        /// </summary>
        /// <param name="ex">The <see cref="Exception"/> to log.</param>
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
                List<string> files = new List<string>(Directory.GetFiles(Path.GetDirectoryName(LogPath), "*.txt"));
                files.Sort((x, y) =>
                {
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
