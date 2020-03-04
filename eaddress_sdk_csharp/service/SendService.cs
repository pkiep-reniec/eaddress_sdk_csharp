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
using Exception = System.Exception;

namespace eaddress_sdk_csharp.service
{
    class SendService
    {
        private static SendService __instance = null;

        private Config config;
        private ConfigAga configAga;
        private SecurityService securityService;
        private Utils utils;
        private String path7Zdll;

        private SendService(Config config, ConfigAga configAga, String path7Zdll)
        {
            this.config = config;
            this.configAga = configAga;
            this.securityService = SecurityService.getInstance(config);
            this.utils = Utils.getInstance;
            this.path7Zdll = path7Zdll;
        }

        public static SendService getInstance(Config config, ConfigAga configAga, String path7Zdll)
        {
            if (__instance == null)
            {
                __instance = new SendService(config, configAga, path7Zdll);
            }

            return __instance;
        }

        public async Task<ApiResponse> ProcSingleNotification(Message oMessage)
        {
            ApiResponse result = null;

            try
            {
                string pathDir = utils.CreateTempDir();
                Metadata metadata = createMetadata(oMessage, null, true);

                if (metadata == null)
                {
                    return null;
                }

                FileStream fileSign = await SignMetadata(metadata, pathDir);

                if (fileSign != null)
                {
                    //security service
                    string token = await this.securityService.getToken();

                    if (token != null)
                    {
                        //send
                        string json = JsonConvert.SerializeObject(metadata);
                        oMessage.metadata = json;

                        result = await SendSingle(fileSign, oMessage, token);

                        if (result != null)
                        {
                            Directory.Delete(pathDir, true);

                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }

            return new ApiResponse(Messages.UNEXPECTED_ERROR);
        }

        public async Task<ApiResponse> ProcMassiveNotification(Message oMessage, FileStream file)
        {
            ApiResponse result = null;

            if (this.configAga == null)
            {
                return null;
            }

            try
            {
                string pathDir = utils.CreateTempDir();
                Metadata metadata = createMetadata(oMessage, file, false);

                if (metadata == null)
                {
                    return null;
                }

                FileStream fileSign = await SignMetadata(metadata, pathDir);

                if (fileSign != null)
                {
                    //security service
                    string token = await this.securityService.getToken();

                    if (token != null)
                    {
                        //send
                        string json = JsonConvert.SerializeObject(metadata);
                        oMessage.metadata = json;

                        result = await SendMassive(fileSign, file, oMessage, token);

                        if (result != null)
                        {
                            Directory.Delete(pathDir, true);

                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }

            return result;
        }

        private Metadata createMetadata(Message message, FileStream fileCsv, Boolean single)
        {
            if (!single && fileCsv == null)
            {
                return null;
            }

            DateTime date = DateTime.Now;

            DataPerson oPerson = new DataPerson
            {
                docType = this.config.docType,
                doc = this.config.doc,
                name = this.config.name
            };

            DataApp oApp = new DataApp
            {
                name = this.config.appName,
                clientId = this.config.clientId
            };

            Metadata metadata = new Metadata();
            metadata.issuer = oPerson;
            metadata.application = oApp;
            metadata.subject = message.subject;
            metadata.tag = message.tag;

            if (single)
            {
                String hashMessage = String.Concat(message.subject, message.message);
                var crypt = new System.Security.Cryptography.SHA256Managed();
                var hash = new System.Text.StringBuilder();
                byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(hashMessage));
                foreach (byte theByte in crypto)
                {
                    hash.Append(theByte.ToString("x2"));
                }

                metadata.checksum = hash.ToString();
                metadata.quantity = 1;
            }
            else
            {
                Metadata metadataChecksum = getChecksum(fileCsv);

                if (metadataChecksum == null)
                {
                    return null;
                }

                metadata.checksum = metadataChecksum.checksum;
                metadata.quantity = metadataChecksum.quantity;
            }

            return metadata;
        }

        private Metadata getChecksum(FileStream fileCsv)
        {
            Metadata metadata = new Metadata();

            try
            {
                String data = "";
                String content = "";
                int count = -1;

                StreamReader sr = new StreamReader(fileCsv);

                while ((data = sr.ReadLine()) != null)
                {
                    content += data.Trim();

                    if (data != "")
                    {
                        count++;
                    }
                }

                metadata = new Metadata();
                var crypt = new System.Security.Cryptography.SHA256Managed();
                var hash = new StringBuilder();
                byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(content));
                foreach (byte theByte in crypto)
                {
                    hash.Append(theByte.ToString("x2"));
                }

                metadata.checksum = hash.ToString();
                metadata.quantity = count;

                return metadata;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }

            return metadata;
        }

        private async Task<FileStream> SignMetadata(Metadata metadata, string pathDir)
        {
            FileStream output = null;

            try
            {
                ServicePointManager.Expect100Continue = false;

                string pathFileZip = utils.CreateZip(metadata, this.configAga, pathDir, path7Zdll);

                FileStream fileZip = new FileStream(pathFileZip, FileMode.Open, FileAccess.Read);

                //sign file zip
                using (var formContent = new MultipartFormDataContent())
                {
                    formContent.Add(new StreamContent(fileZip), "uploadFile", fileZip.Name);

                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(configAga.agaUri, formContent);

                        if (response.IsSuccessStatusCode)
                        {
                            client.Dispose();
                            Stream fileSign = response.Content.ReadAsStreamAsync().Result;

                            using (output = new FileStream(string.Concat(pathDir, Constants.FILE_SIGN), FileMode.Create))
                            {
                                fileSign.CopyTo(output);
                            }

                            return output;
                        }
                        else
                        {
                            Debug.WriteLine(response.RequestMessage.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }

            return output;
        }

        private async Task<ApiResponse> SendSingle(FileStream file, Message oMessage, string token)
        {
            try
            {
                ServicePointManager.Expect100Continue = false;

                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.AUTHORIZATION_BEARER, token);

                FileStream fileSign = new FileStream(file.Name, FileMode.Open);

                using (var form = new MultipartFormDataContent())
                {
                    form.Add(new StringContent(oMessage.doc), "doc");
                    form.Add(new StringContent(oMessage.docType), "docType");
                    form.Add(new StringContent(oMessage.subject), "subject");
                    form.Add(new StringContent(oMessage.message), "message");
                    form.Add(new StringContent(oMessage.rep == null ? "" : oMessage.rep), "rep");
                    form.Add(new StringContent(oMessage.tag == null ? "" : oMessage.tag), "tag");
                    form.Add(new StringContent(oMessage.callback == null ? "" : oMessage.callback), "callback");
                    form.Add(new StringContent(oMessage.attachments == null ? "" : oMessage.attachments), "attachments");
                    form.Add(new StringContent(oMessage.metadata), "metadata");
                    form.Add(new StreamContent(fileSign), "fileSign", fileSign.Name);

                    HttpResponseMessage response = await httpClient.PostAsync(string.Concat(this.config.eaddressServiceUri, "/api/v1.0/send/single"), form);
                    
                    httpClient.Dispose();
                    String content = await response.Content.ReadAsStringAsync();
                    ApiResponse tokenResponse = JsonConvert.DeserializeObject<ApiResponse>(content);

                    return tokenResponse;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }

            return null;
        }

        private async Task<ApiResponse> SendMassive(FileStream file, FileStream fileCSV, Message oMessage, string accessToken)
        {
            try
            {
                ServicePointManager.Expect100Continue = false;
                fileCSV.Close();

                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.AUTHORIZATION_BEARER, accessToken);

                FileStream fileDataSign = new FileStream(file.Name, FileMode.Open);
                FileStream fileDataCSV = new FileStream(fileCSV.Name, FileMode.Open);

                using (var form = new MultipartFormDataContent())
                {
                    form.Add(new StringContent(oMessage.subject), "subject");
                    form.Add(new StringContent(oMessage.message), "message");
                    form.Add(new StringContent(oMessage.tag == null ? "" : oMessage.tag), "tag");
                    form.Add(new StringContent(oMessage.callback == null ? "" : oMessage.callback), "callback");
                    form.Add(new StringContent(oMessage.metadata), "metadata");
                    form.Add(new StreamContent(fileDataCSV), "fileCSV", fileDataCSV.Name);
                    form.Add(new StreamContent(fileDataSign), "fileSign", fileDataSign.Name);

                    HttpResponseMessage response = await httpClient.PostAsync(string.Concat(this.config.eaddressServiceUri, "/api/v1.0/send/massive"), form);

                    if (response.IsSuccessStatusCode)
                    {
                        httpClient.Dispose();
                        String content = await response.Content.ReadAsStringAsync();
                        ApiResponse tokenResponse = JsonConvert.DeserializeObject<ApiResponse>(content);

                        return tokenResponse;
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
