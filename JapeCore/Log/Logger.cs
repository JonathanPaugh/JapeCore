using System;
using System.IO;

namespace JapeCore
{
    public class Logger
    {
        internal FileStream Stream { get; }
        internal StreamWriter Writer { get; }

        internal Logger(string file)
        {   
            string filePath = Path.Combine(JapeCore.Log.LogPath, $"{file ?? string.Empty}");

            try
            {
                Directory.CreateDirectory(JapeCore.Log.LogPath);
                Stream = new FileStream(filePath, FileMode.Append, FileAccess.Write);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Cannot open {filePath} for writing");
                Console.WriteLine(e.Message);
                return;
            }

            try
            {
                Writer = new StreamWriter(Stream);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Cannot create stream writer for {filePath}");
                Console.WriteLine(e.Message);
                return;
            }

            Writer.AutoFlush = true;
        }

        public void Write(string value)
        {
            JapeCore.Log.WriteConsole(value);
            Log(value);
        }

        public void Log(string value)
        {
            try
            {
                Writer.WriteLine(JapeCore.Log.Stamp(value));
            }
            catch (Exception)
            {
                Console.WriteLine("Could not write to log");
            }
        }
    }
}
