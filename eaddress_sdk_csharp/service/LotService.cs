using eaddress_sdk_csharp.common;
using eaddress_sdk_csharp.dto;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace eaddress_sdk_csharp.service
{
    class LotService
    {
        private static LotService __instance = null;

        private Config config;
        private SecurityService securityService;
        private Utils utils;

        private LotService(Config config)
        {
            this.config = config;
            this.securityService = SecurityService.getInstance(config);
            this.utils = Utils.getInstance;
        }

        public static LotService getInstance(Config config)
        {
            if (__instance == null)
            {
                __instance = new LotService(config);
            }

            return __instance;
        }

        public async Task<PaginatorLot> FetchAll(SearchRequest searchRequest)
        {
            try
            {
                ServicePointManager.Expect100Continue = false;

                string token = await this.securityService.getToken();

                if (token != null)
                {
                    UriBuilder uriBuilder = utils.ConvertToUriBuilder(searchRequest, string.Concat(this.config.eaddressServiceUri, "/api/v1.0/lot"));

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
                        PaginatorLot paginatorLot = JsonConvert.DeserializeObject<PaginatorLot>(content);

                        return paginatorLot;
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

        public async Task<PaginatorLotNotifications> FetchNotifications(string lotId, SearchRequest searchRequest)
        {
            try
            {
                ServicePointManager.Expect100Continue = false;

                string token = await this.securityService.getToken();

                if (token != null)
                {
                    UriBuilder uriBuilder = utils.ConvertToUriBuilder(searchRequest, string.Concat(this.config.eaddressServiceUri, "/api/v1.0/lot", lotId));

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
                        PaginatorLotNotifications paginatorLotNotifications = JsonConvert.DeserializeObject<PaginatorLotNotifications>(content);

                        return paginatorLotNotifications;
                    }
                }
            } catch (Exception ex) {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }

            return null;
        }

        public async Task<byte[]> DownloadMetadata(string lotId)
        {
            try
            {
                ServicePointManager.Expect100Continue = false;

                string token = await this.securityService.getToken();

                if (token != null)
                {
                    HttpClient myHttpClient = new HttpClient();
                    myHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.AUTHORIZATION_BEARER, token);

                    HttpResponseMessage response = await myHttpClient.GetAsync(string.Concat(this.config.eaddressServiceUri, "/api/v1.0/lot", lotId, "/metadata"));

                    if (response.IsSuccessStatusCode)
                    {
                        byte[] content = response.Content.ReadAsByteArrayAsync().Result;

                        return content;
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
