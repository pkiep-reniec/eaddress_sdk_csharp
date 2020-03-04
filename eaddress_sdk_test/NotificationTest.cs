using eaddress_sdk_csharp;
using eaddress_sdk_csharp.common;
using eaddress_sdk_csharp.dto;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace eaddress_sdk_test
{
    [TestClass]
    public class NotificationTest
    {
        private string tempDir = Path.GetTempPath();
        private ReniecEAddressClient reniecEAddressClient;
        private string notificationId;
        private string lotId;

        [TestMethod]
        public void FetchAllNotificationsTest()
        {
            Before();

            SearchRequest searchRequest = new SearchRequest();
            //searchRequest.page=1;
            //searchRequest.count=5;
            //searchRequest.doc="46256479";
            //searchRequest.name="alexander";
            //searchRequest.subject="prueba";
            //searchRequest.dateBegin=1580515200L;
            //searchRequest.dateEnd=1583020799L;

            Task<ApiPaginatorLotNotifications> notifications = reniecEAddressClient.FetchAllNotifications(searchRequest);
            notifications.Wait();

            Console.WriteLine(notifications.Result.recordsTotal);

            foreach (NotificationsResponse notification in notifications.Result.data)
            {
                Console.WriteLine(JsonConvert.SerializeObject(notification));
            }
            
            Assert.IsTrue(notifications.Result.recordsTotal > 0);
        }

        [TestMethod]
        public void GetNotificationTest()
        {
            Before();

            Task<NotificationsResponse> notification = reniecEAddressClient.GetNotification(notificationId, lotId);
            notification.Wait();

            Console.WriteLine(JsonConvert.SerializeObject(notification.Result));

            Assert.IsNotNull(notification.Result);
        }

        [TestMethod]
        public void DownloadAcuseTest()
        {
            Before();

            try
            {
                Task<byte[]> acuse = reniecEAddressClient.DownloadAcuse(notificationId, lotId, Constants.RECEIVED);
                acuse.Wait();

                if (acuse != null)
                {
                    using (var output = new FileStream(string.Concat(tempDir, "acuse.zip"), FileMode.Create))
                    {
                        Stream stream = new MemoryStream(acuse.Result);
                        stream.CopyTo(output);
                    }
                }

                Assert.IsNotNull(acuse);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }
        }

        public void Before()
        {
            string configFile = @"..\..\resources\config.json";
            reniecEAddressClient = new ReniecEAddressClient(configFile);

            this.notificationId = "5e5e79a4c89b4704dca0fd90";
            this.lotId = "5e5e79a0c89b470437b5ad21";
        }
    }
}
