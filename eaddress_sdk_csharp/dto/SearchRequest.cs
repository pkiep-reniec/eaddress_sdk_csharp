using System;

namespace eaddress_sdk_csharp.dto
{
    public class SearchRequest
    {
        public int page { get; set; }
        public int count { get; set; }
        public string sort { get; set; }
        public string tag { get; set; }
        public string name { get; set; }
        public string doc { get; set; }
        public string subject { get; set; }
        public int status { get; set; }
        public long dateBegin { get; set; }
        public long dateEnd { get; set; }
        public bool failed { get; set; }

        public SearchRequest()
        {
            this.page = 1;
            this.count = 20;

        }

    }
}
