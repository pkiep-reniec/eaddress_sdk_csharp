using Newtonsoft.Json;
using System;

namespace eaddress_sdk_csharp.dto
{
    public class Metadata
    {
        [JsonProperty("emisor")]
        public DataPerson issuer { get; set; }
        [JsonProperty("delegado")]
        public DataPerson delegates { get; set; }
        [JsonProperty("asunto")]
        public String subject { get; set; }
        [JsonProperty("tag")]
        public String tag { get; set; }
        [JsonProperty("cantidad")]
        public Int32 quantity { get; set; }
        [JsonProperty("aplicacion")]
        public DataApp application { get; set; }
        [JsonProperty("checksum")]
        public String checksum { get; set; }
    }
}
