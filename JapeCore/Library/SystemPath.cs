using System;
using System.IO;

namespace JapeCore
{
    public static class SystemPath
    {
        public static string Format(string path)
        {
            path = path.Replace('/', Path.DirectorySeparatorChar);
            path = path.Replace('\\', Path.DirectorySeparatorChar);
            return Path.TrimEndingDirectorySeparator(path);
        }
    }
}
