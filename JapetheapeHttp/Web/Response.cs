using System;
using System.IO;
using System.Net;
using System.Text;

namespace JapeHttp
{
    public class Response
    {
        private string value;

        public Response(HttpWebRequest request)
        {
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream())) 
                {
                    value = reader.ReadToEnd();
                }
            }
        }

        public string Read() { return value; }
    }
}
