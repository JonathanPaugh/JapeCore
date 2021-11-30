using System;
using System.Diagnostics;
using System.IO;

namespace JapeCore
{
    public static class Log
    {
        public static Logger Create(string file) => new(file);

        internal static string LogPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");

        private static Logger defaultLogger;

        private static ConsoleTraceListener consoleListener;
        private static TextWriterTraceListener logListener;

        public static void InitDefault(string file = null)
        {
            Trace.AutoFlush = true;

            Trace.Listeners.Clear();

            defaultLogger = new Logger(file);

            consoleListener = new ConsoleTraceListener(true);

            logListener = new TextWriterTraceListener(defaultLogger.Stream)
            {
                TraceOutputOptions = TraceOptions.ThreadId
            };

            Trace.Listeners.Add(consoleListener);
            Trace.Listeners.Add(logListener);

            AppDomain.CurrentDomain.UnhandledException += delegate(object _, UnhandledExceptionEventArgs e)
            {
                Write(e.ExceptionObject.ToString());
            };
        }

        /// <summary>
        /// Writes to console and to the default log
        /// </summary>
        /// <param name="value"></param>
        public static void Write(string value)
        {
            Trace.WriteLine(Stamp(value));
        }

        /// <summary>
        /// Writes to the console
        /// </summary>
        /// <param name="value"></param>
        public static void WriteConsole(string value)
        {
            Trace.Listeners.Remove(logListener);
            Write(value);
            Trace.Listeners.Add(logListener);
        }

        /// <summary>
        /// Writes to the default log
        /// </summary>
        /// <param name="value"></param>
        public static void WriteLog(string value)
        {
            try
            {
                defaultLogger.Log(Stamp(value));
            }
            catch (Exception)
            {
                Console.WriteLine("Could not write to log");
            }
        }
        
        public static string Stamp(string value) => $"{Timestamp()}: {value}";
        public static string Timestamp() => $"{DateTime.UtcNow:u}";
    }
}
