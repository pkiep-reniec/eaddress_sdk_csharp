using System.Collections.Generic;

namespace eaddress_sdk_csharp.dto
{
    public class ApiPaginatorLotNotifications
    {
        public int recordsTotal { get; set; }
        public List<NotificationsResponse> data { get; set; }

        public ApiPaginatorLotNotifications()
        {
            this.recordsTotal = 0;
            this.data = new List<NotificationsResponse>();
        }
    }
}
