using Newtonsoft.Json;
using System;

namespace eaddress_sdk_csharp.dto
{
    public class DataApp
    {
        [JsonProperty("nombre")]
        public String name { get; set; }
        [JsonProperty("identificador")]
        public String clientId { get; set; }
    }
}
