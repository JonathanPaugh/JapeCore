using System;
using System.IO;
using System.Threading.Tasks;
using JapeCore;

namespace JapeWeb
{
    public class Templater
    {
        private readonly string path;
        private readonly Func<string, Task<string>> readFile;

        internal Templater(string path, Func<string, Task<string>> readFile)
        {
            this.path = path;
            this.readFile = readFile;
        }

        public async Task<string> GetTemplate(string path)
        {
            return await readFile.Invoke(Path.Combine(this.path, VirtualPath.Format(path)));
        }
    }
}
