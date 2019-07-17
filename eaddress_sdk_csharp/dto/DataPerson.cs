using Newtonsoft.Json;
using System;

namespace eaddress_sdk_csharp.dto
{
    public class DataPerson
    {
        [JsonProperty("tipo_documento")]
        public String docType { get; set; }
        [JsonProperty("documento")]
        public String doc { get; set; }
        [JsonProperty("nombre")]
        public String name { get; set; }
    }
}
