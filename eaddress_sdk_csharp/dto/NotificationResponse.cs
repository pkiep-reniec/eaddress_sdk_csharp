using System.Collections.Generic;

namespace eaddress_sdk_csharp.dto
{
    public class NotificationResponse
    {
        public string id { get; set; }
        public string lotId { get; set; }
        public string tag { get; set; }
        public string issuerDoc { get; set; }
        public string issuerDocType { get; set; }
        public string issuerName { get; set; }
        public string issuerNameShort { get; set; }
        public string issuerIcon { get; set; }
        public string receiverDoc { get; set; }
        public string receiverDocType { get; set; }
        public string receiverName { get; set; }
        public int? serviceId { get; set; }
        public string subject { get; set; }
        public string messageHtml { get; set; }
        public bool important { get; set; }
        public long sentAt { get; set; }
        public string jsonSent { get; set; }
        public string jsonSentSignature { get; set; }
        public long receivedAt { get; set; }
        public string jsonReceived { get; set; }
        public string jsonReceivedSignature { get; set; }
        public long readAt { get; set; }
        public int? readCount { get; set; }
        public string code { get; set; }
        public string errorReason { get; set; }
        public List<Attachment> attachments { get; set; }
        public long createdAt { get; set; }

        public NotificationResponse()
        {
        }

    }
}
