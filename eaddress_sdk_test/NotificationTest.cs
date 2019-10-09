﻿using eaddress_sdk_csharp;
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

            SearchRequest searchRequest = new SearchRequest
            {
                page = 1,   
                count = 5,
                name = "",
            };

            Task<ApiPaginatorLotNotifications> notifications = reniecEAddressClient.FetchAllNotifications(searchRequest);
            notifications.Wait();

            foreach (NotificationsResponse notification in notifications.Result.notifications)
            {
                Console.WriteLine(JsonConvert.SerializeObject(notification));
            }

            Console.WriteLine(notifications.Result.recordsTotal);
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

            this.notificationId = "5d8d2a39c89b471284d34e36";
            this.lotId = "5d8d2a36c89b4711655603e5";
        }
    }
}
