using System.Collections.Generic;

namespace eaddress_sdk_csharp.dto
{
    public class ApiPaginatorLotNotifications
    {
        public int recordsTotal { get; set; }
        public List<NotificationsResponse> notifications { get; set; }

        public ApiPaginatorLotNotifications()
        {
            this.recordsTotal = 0;
            this.notifications = new List<NotificationsResponse>();
        }
    }
}
