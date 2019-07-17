using System.Collections.Generic;

namespace eaddress_sdk_csharp.dto
{
    public class PaginatorLotNotifications
    {
        public int recordsTotal { get; set; }
        public List<NotificationResponse> notifications { get; set; }

        public PaginatorLotNotifications()
        {
            this.recordsTotal = 0;
            this.notifications = new List<NotificationResponse>();
        }
    }
}
