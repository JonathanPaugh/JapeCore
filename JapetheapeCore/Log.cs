using System;
using System.Diagnostics;
using System.IO;

namespace JapeCore
{
    public class Log
    {
        private static string LogPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        private static FileStream stream;

        public static void Init(string logFile = null)
        {
            Trace.Listeners.Clear();

            try
            {
                Directory.CreateDirectory(LogPath);
                stream = new FileStream(Path.Combine(LogPath, $"{logFile ?? string.Empty}.log"), FileMode.Append, FileAccess.Write);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Cannot open {LogPath} for writing");
                Console.WriteLine(e.Message);
                return;
            }

            Trace.Listeners.Add(new ConsoleTraceListener(true));

            Trace.Listeners.Add(new TextWriterTraceListener(stream)
            {
                TraceOutputOptions = TraceOptions.ThreadId
            });

            Trace.AutoFlush = true;

            AppDomain.CurrentDomain.UnhandledException += delegate(object sender, UnhandledExceptionEventArgs e)
            {
                Write(e.ExceptionObject.ToString());
            };
        }

        public static void Write(string value)
        {
            Trace.WriteLine(Stamp(value));
        }

        public static void WriteLog(string value)
        {
            StreamWriter writer = new(stream);
            writer.WriteLine(Stamp(value));
            writer.Close();
        }
        
        public static string Stamp(string value)
        {
            return $"{DateTime.UtcNow:u}: {value}";
        }
    }
}
