using System;

namespace eaddress_sdk_csharp.dto
{
    public class Attachment
    {
        public string name { get; set; }
        public string url { get; set; }

        public Attachment()
        {
        }

        public Attachment(string name, string url)
        {
            this.name = name;
            this.url = url;
        }
    }
}
