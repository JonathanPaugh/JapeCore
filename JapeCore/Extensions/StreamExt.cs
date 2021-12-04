using System.IO;

namespace JapeCore
{
    public static class StreamExt
    {
        public static void Rewind(this Stream stream)
        {
            stream.Position = 0;
        }
    }
}
