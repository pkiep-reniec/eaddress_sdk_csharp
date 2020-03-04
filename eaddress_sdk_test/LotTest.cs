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
            //searchRequest.page=1;
            //searchRequest.count=20;
            //searchRequest.dateBegin=1580515200L;
            //searchRequest.dateEnd=1583020799L;

            Task<PaginatorLot> paginatorLot = reniecEAddressClient.FetchAllLots(searchRequest);
            paginatorLot.Wait();

            Console.WriteLine(paginatorLot.Result.recordsTotal);

            foreach (LotData lotData in paginatorLot.Result.data)
            {
                Console.WriteLine(JsonConvert.SerializeObject(lotData));
            }

            Assert.IsTrue(paginatorLot.Result.recordsTotal > 0);
        }

        [TestMethod]
        public void FetchLotNotificationsTest()
        {
            Before();

            SearchRequest searchRequest = new SearchRequest();
            //searchRequest.page = 1;
            //searchRequest.count = 20;
            //searchRequest.status = null;
            //searchRequest.dateBegin=1580515200L;
            //searchRequest.dateEnd=1583020799L;

            Task<ApiPaginatorLotNotifications> paginatorLotNotifications = reniecEAddressClient.FetchLotNotifications(lotId, searchRequest);
            paginatorLotNotifications.Wait();

            Console.WriteLine(paginatorLotNotifications.Result.recordsTotal);

            foreach (NotificationsResponse notificationResponse in paginatorLotNotifications.Result.data)
            {
                Console.WriteLine(JsonConvert.SerializeObject(notificationResponse));
            }
            
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

            this.lotId = "5e5e79a0c89b470437b5ad21";
        }
    }
}
