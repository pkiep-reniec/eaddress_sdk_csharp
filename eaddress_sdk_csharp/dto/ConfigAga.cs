using Newtonsoft.Json;
using System;
using System.Collections.Generic;

/**
 * @author Alexander Llacho
 */
namespace eaddress_sdk_csharp.dto
{
    public class ConfigAga
    {
        [JsonProperty("aga_uri")]
        public String agaUri { get; set; }
        public String timestamp { get; set; }
        public String certificateId { get; set; }
        public String secretPassword { get; set; }
    }
}
