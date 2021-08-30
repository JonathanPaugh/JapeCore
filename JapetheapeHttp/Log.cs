using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Text;

namespace JapeHttp
{
    public class Log
    {
        private const string LogFile = "log.txt";

        private static FileStream stream;

        public static void Init()
        {
            Trace.Listeners.Clear();

            try
            {
                stream = new FileStream($"./{LogFile}", FileMode.Append, FileAccess.Write);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Cannot open {LogFile} for writing");
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
                Write(e.ToString());
            };
        }

        public static void Write(string value)
        {
            Trace.WriteLine(Stamp(value));
        }

        public static void WriteLog(string value)
        {
            StreamWriter writer = new StreamWriter(stream);
            writer.WriteLine(Stamp(value));
            writer.Close();
        }
        
        public static string Stamp(string value)
        {
            return $"{DateTime.UtcNow:u}: {value}";
        }
    }
}
