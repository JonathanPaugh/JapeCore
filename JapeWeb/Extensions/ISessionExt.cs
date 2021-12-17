using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace JapeWeb
{
    public static class ISessionExt
    {
        public static bool TryGetString(this ISession session, string key, out string value)
        {
            value = null;

            if (!session.IsAvailable) { return false; }

            value = session.GetString(key);
            return value != null;
        } 
    }
}
