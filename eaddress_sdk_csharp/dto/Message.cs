using Newtonsoft.Json;
using System;

/**
 * @author Alexander Llacho
 */
namespace eaddress_sdk_csharp.dto
{
    public class Message
    {
        [JsonProperty("doc")]
        public String doc { get; set; }
        [JsonProperty("doc_type")]
        public String docType { get; set; }
        [JsonProperty("subject")]
        public String subject { get; set; }
        [JsonProperty("message")]
        public String message { get; set; }
        [JsonProperty("rep")]
        public String rep { get; set; }
        [JsonProperty("tag")]
        public String tag { get; set; }
        [JsonProperty("callback")]
        public String callback { get; set; }
        [JsonProperty("attachments")]
        public String attachments { get; set; }
        [JsonProperty("metadata")]
        public String metadata { get; set; }
    }
}
