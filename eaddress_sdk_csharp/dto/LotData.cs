using System;

namespace eaddress_sdk_csharp.dto
{
    public class LotData
    {
        public string id { get; set; }
        public string tag { get; set; }
        public string subject { get; set; }
        public int quantity { get; set; }
        public int maximumQuantity { get; set; }
        public int errors { get; set; }
        public string doc { get; set; }
        public string docType { get; set; }
        public string receiver { get; set; }
        public long createdAt { get; set; }

    }
}
