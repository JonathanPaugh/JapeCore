using System;
using System.Threading.Tasks;
using JapeCore;

namespace JapeWeb
{
    public class Templater : IWebComponent
    {
        public string Path { get; }

        private readonly Func<string, Task<string>> readFile;

        internal Templater(string path, Func<string, Task<string>> readFile)
        {
            Path = path;
            this.readFile = readFile;
        }

        public async Task<string> ReadFile(string path)
        {
            return await readFile.Invoke(System.IO.Path.Combine(Path, SystemPath.Format(path)));
        }
    }
}
