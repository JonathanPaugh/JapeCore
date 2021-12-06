using System;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace JapeCore
{
    public static class PathStringExt
    {
        private const char Seperator = '/';

        private static PathString[] Convert(string[] paths)
        {
            PathString[] temp = new PathString[paths.Length];
            for (int i = 0; i < paths.Length; i++)
            {
                temp[i] = Seperator + paths[i];
            }
            return temp;
        }

        public static PathString[] Split(this PathString path, int count, StringSplitOptions options = StringSplitOptions.None)
        {
            string[] paths = path.ToString().Split(Seperator, count, options).ToArray();
            return Convert(paths);
        }

        public static PathString[] Split(this PathString path, StringSplitOptions options = StringSplitOptions.None)
        {
            string[] paths = path.ToString().Split(Seperator, options).ToArray();
            return Convert(paths);
        }

        public static PathString Pop(this PathString path, out PathString remaining)
        {
            remaining = PathString.Empty;
            PathString[] paths = path.Split(2, StringSplitOptions.RemoveEmptyEntries);
            switch (paths.Length)
            {
                case > 2: throw new Exception("Internal Logic Error");
                case > 1: remaining = paths[1]; break;
            }

            return paths[0];
        }
    }
}
