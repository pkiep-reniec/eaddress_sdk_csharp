using eaddress_sdk_csharp.dto;
using eaddress_sdk_csharp.service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace eaddress_sdk_csharp
{
    public class ReniecEAddressClient
    {
        private Config config;
        private ConfigAga configAga = null;
        private SendService sendService;
        private LotService lotService;
        private NotificationService notificationService;

        public ReniecEAddressClient(string configFile, ConfigAga oConfigAga)
        {
            this.setConfig(configFile, oConfigAga);
        }

        public ReniecEAddressClient(string configFile)
        {
            this.setConfig(configFile, null);
        }

        public async Task<ApiResponse> SendSingleNotification(Message oMessage)
        {
            return await this.SendSingleNotification(oMessage, null);
        }

        public async Task<ApiResponse> SendSingleNotification(Message oMessage, List<Attachment> attachments)
        {
            if (this.configAga == null)
            {
                return null;
            }

            try
            {
                if (attachments != null)
                {
                    string jsonAttachments = JsonConvert.SerializeObject(attachments);
                    oMessage.attachments = jsonAttachments;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }

            return await this.sendService.ProcSingleNotification(oMessage);
        }

        public async Task<ApiResponse> SendMassiveNotification(Message oMessage, FileStream file)
        {
            if (this.configAga == null)
            {
                return null;
            }

            return await this.sendService.ProcMassiveNotification(oMessage, file);
        }

        public Task<PaginatorLot> FetchAllLots(SearchRequest searchRequest)
        {
            return this.lotService.FetchAll(searchRequest);
        }

        public Task<PaginatorLotNotifications> FetchLotNotifications(string lotId, SearchRequest searchRequest)
        {
            return this.lotService.FetchNotifications(lotId, searchRequest);
        }

        public Task<PaginatorLotNotifications> FetchAllNotifications(SearchRequest searchRequest)
        {
            return this.notificationService.FetchAll(searchRequest);
        }

        public Task<NotificationResponse> GetNotification(String notificationId, String lotId)
        {
            return this.notificationService.GetOne(notificationId, lotId);
        }

        public Task<byte[]> DownloadMetadata(string lotId)
        {
            return this.lotService.DownloadMetadata(lotId);
        }

        public Task<byte[]> DownloadAcuse(string id, string lotId, string acuse)
        {
            return this.notificationService.DownloadAcuse(id, lotId, acuse);
        }

        private void setConfig(string configFile, ConfigAga oConfigAga)
        {
            try
            {
                using (StreamReader file = File.OpenText(@configFile))
                {
                    JsonSerializer jsonConfig = new JsonSerializer();
                    this.config = (Config)jsonConfig.Deserialize(file, typeof(Config));
                }

                this.configAga = oConfigAga;
                this.sendService = SendService.getInstance(this.config, this.configAga);
                this.lotService = LotService.getInstance(this.config);
                this.notificationService = NotificationService.getInstance(this.config);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }
        }
    }
}
