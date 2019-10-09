using eaddress_sdk_csharp;
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
    public class LotTest
    {
        private string tempDir = Path.GetTempPath();
        private ReniecEAddressClient reniecEAddressClient;
        private string lotId;

        [TestMethod]
        public void FetchAllLotsTest()
        {
            Before();

            SearchRequest searchRequest = new SearchRequest();

            Task<PaginatorLot> paginatorLot = reniecEAddressClient.FetchAllLots(searchRequest);
            paginatorLot.Wait();

            foreach (LotData lotData in paginatorLot.Result.data)
            {
                Console.WriteLine(JsonConvert.SerializeObject(lotData));
            }

            Console.WriteLine(paginatorLot.Result.recordsTotal);
            Assert.IsTrue(paginatorLot.Result.recordsTotal > 0);
        }

        [TestMethod]
        public void FetchLotNotificationsTest()
        {
            Before();

            SearchRequest searchRequest = new SearchRequest();
            searchRequest.page = 1;
            searchRequest.count = 20;

            Task<ApiPaginatorLotNotifications> paginatorLotNotifications = reniecEAddressClient.FetchLotNotifications(lotId, searchRequest);
            paginatorLotNotifications.Wait();

            foreach (NotificationsResponse notificationResponse in paginatorLotNotifications.Result.notifications)
            {
                Console.WriteLine(JsonConvert.SerializeObject(notificationResponse));
            }

            Console.WriteLine(paginatorLotNotifications.Result.recordsTotal);
            Assert.IsTrue(paginatorLotNotifications.Result.recordsTotal > 0);
        }

        [TestMethod]
        public void DownloadMetadataTest()
        {
            Before();

            try
            {
                Task<byte[]> metadata = reniecEAddressClient.DownloadMetadata(lotId);
                metadata.Wait();

                if (metadata != null)
                {
                    using (var output = new FileStream(string.Concat(tempDir, "metadata.zip"), FileMode.Create))
                    {
                        Stream stream = new MemoryStream(metadata.Result);
                        stream.CopyTo(output);
                    }
                }

                Assert.IsNotNull(metadata);
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

            this.lotId = "5d8d2a36c89b4711655603e5";
        }
    }
}
