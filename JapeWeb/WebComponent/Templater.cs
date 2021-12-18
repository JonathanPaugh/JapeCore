using System;
using System.Threading.Tasks;
using JapeCore;

namespace JapeWeb
{
    public class Templater
    {
        public string Path { get; }

        private readonly Func<string, string> readFile;
        private readonly Func<string, Task<string>> readFileAsync;

        internal Templater(string path, Func<string, string> readFile, Func<string, Task<string>> readFileAsync)
        {
            Path = path;
            this.readFile = readFile;
            this.readFileAsync = readFileAsync;
        }

        public string GetRequestPath(string path) => System.IO.Path.Combine(Path, SystemPath.Format(path));

        public string ReadFile(string path) => readFile.Invoke(GetRequestPath(path));
        public async Task<string> ReadFileAsync(string path) => await readFileAsync.Invoke(GetRequestPath(path));
    }
}
