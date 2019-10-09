using eaddress_sdk_csharp.common;
using eaddress_sdk_csharp.dto;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace eaddress_sdk_csharp.service
{
    class NotificationService
    {
        private static NotificationService __instance = null;

        private Config config;
        private SecurityService securityService;
        private Utils utils;

        private NotificationService(Config config)
        {
            this.config = config;
            this.securityService = SecurityService.getInstance(config);
            this.utils = Utils.getInstance;
        }

        public static NotificationService getInstance(Config config)
        {
            if (__instance == null)
            {
                __instance = new NotificationService(config);
            }

            return __instance;
        }

        public async Task<ApiPaginatorLotNotifications> FetchAll(SearchRequest searchRequest)
        {
            try
            {
                ServicePointManager.Expect100Continue = false;

                string token = await this.securityService.getToken();

                if (token != null)
                {
                    UriBuilder uriBuilder = utils.ConvertToUriBuilder(searchRequest, string.Concat(this.config.eaddressServiceUri, "/api/v1.0/notification"));

                    if (uriBuilder == null)
                    {
                        return null;
                    }

                    HttpClient myHttpClient = new HttpClient();
                    myHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.AUTHORIZATION_BEARER, token);

                    HttpResponseMessage response = await myHttpClient.GetAsync(uriBuilder.ToString());

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        ApiPaginatorLotNotifications paginatorLotNotifications = JsonConvert.DeserializeObject<ApiPaginatorLotNotifications>(content);

                        return paginatorLotNotifications;
                    }
                    else
                    {
                        Debug.WriteLine(response.RequestMessage.ToString());
                    }
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }

            return null;
        }

        public async Task<NotificationsResponse> GetOne(string id, string lotId)
        {
            try
            {
                ServicePointManager.Expect100Continue = false;

                string token = await this.securityService.getToken();

                if (token != null)
                {
                    HttpClient myHttpClient = new HttpClient();
                    myHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.AUTHORIZATION_BEARER, token);
                    string values = string.Concat(this.config.eaddressServiceUri, "/api/v1.0/notification/", id, "/", lotId);

                    HttpResponseMessage response = await myHttpClient.GetAsync(string.Concat(this.config.eaddressServiceUri, "/api/v1.0/notification/", id, "/", lotId));

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        NotificationsResponse notificationResponse = JsonConvert.DeserializeObject<NotificationsResponse>(content);

                        return notificationResponse;
                    }
                    else
                    {
                        Debug.WriteLine(response.RequestMessage.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }

            return null;
        }

        public async Task<byte[]> DownloadAcuse(string id, string lotId, string acuse)
        {
            try
            {
                ServicePointManager.Expect100Continue = false;

                string token = await this.securityService.getToken();

                if (token != null)
                {
                    HttpClient myHttpClient = new HttpClient();
                    myHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.AUTHORIZATION_BEARER, token);

                    HttpResponseMessage response = await myHttpClient.GetAsync(string.Concat(this.config.eaddressServiceUri, "/api/v1.0/notification", id, "/", lotId, "/", acuse));

                    if (response.IsSuccessStatusCode)
                    {
                        byte[] content = response.Content.ReadAsByteArrayAsync().Result;

                        return content;
                    }
                    else
                    {
                        Debug.WriteLine(response.RequestMessage.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }

            return null;
        }
    }
}
