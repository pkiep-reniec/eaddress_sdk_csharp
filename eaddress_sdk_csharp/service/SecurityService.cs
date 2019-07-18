using eaddress_sdk_csharp.dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Exception = System.Exception;

namespace eaddress_sdk_csharp.service
{
    public class SecurityService
    {
        private static SecurityService __insstance = null;

        private FileStream tempFile;
        private Config config;

        private SecurityService(Config config)
        {
            this.config = config;
            this.tempFile = new FileStream(string.Concat(Path.GetTempPath(), @"reniec_eaddress_token.txt"),FileMode.OpenOrCreate);
            this.tempFile.Close();
        }

        public static SecurityService getInstance(Config config)
        {
            if (__insstance == null)
            {
                __insstance = new SecurityService(config);
            }

            return __insstance;
        }

        public async Task<String> getToken()
        {
            string token = ReadToken();

            if (!String.IsNullOrEmpty(token))
            {
                if (ValidateToken(token))
                {
                    return token;
                }
            }

            try
            {
                ServicePointManager.Expect100Continue = false;

                FormUrlEncodedContent formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "client_credentials"),
                    new KeyValuePair<string, string>("scope", "eaddress"),
                    new KeyValuePair<string, string>("client_id", this.config.clientId),
                    new KeyValuePair<string, string>("client_secret", this.config.clientSecret),
                });

                HttpClient myHttpClient = new HttpClient();

                var response = await myHttpClient.PostAsync(this.config.securityUri, formContent);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    TokenResponse tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(content);
                    SaveToken(tokenResponse.accessToken);

                    return tokenResponse.accessToken;
                }
                else
                {
                    Debug.WriteLine(response.RequestMessage.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }

            return null;
        }

        private void SaveToken(string token)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(new FileStream(tempFile.Name, FileMode.Open)))
                {
                    writer.Write(token);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }
        }

        private string ReadToken()
        {
            try
            {
                if (!File.Exists(tempFile.Name))
                {
                    return null;
                }

                byte[] encoded = File.ReadAllBytes(Path.GetFullPath(tempFile.Name));

                return Encoding.UTF8.GetString(encoded);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }

            return null;
        }

        private bool ValidateToken(string token)
        {
            string[] parts = token.Split('.');

            try
            {
                if (parts.Length == 3)
                {
                    byte[] bytes = Encoding.ASCII.GetBytes(parts[1]);
                    string decoded = Encoding.UTF8.GetString(bytes);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }

            return false;
        }
    }
}
