using System;
using System.IO;

namespace JapeCore
{
    public static class VirtualPath
    {
        public static string Format(string path)
        {
            path = path.Replace('/', Path.DirectorySeparatorChar);
            path = path.Replace('\\', Path.DirectorySeparatorChar);
            return Path.TrimEndingDirectorySeparator(path);
        }
    }
}
