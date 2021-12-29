using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.IO;

namespace JapeDatabase
{
    internal static class Bson
    {
        internal static JsonWriterSettings CanonicalJson => new()
        {
            OutputMode = JsonOutputMode.CanonicalExtendedJson,
        };
    }
}
