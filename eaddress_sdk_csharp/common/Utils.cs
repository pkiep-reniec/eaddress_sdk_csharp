using eaddress_sdk_csharp.dto;
using Newtonsoft.Json;
using SevenZip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Web;

/**
 * @author Alexander Llacho
 */
namespace eaddress_sdk_csharp.common
{
    public class Utils
    {
        private static Utils __instance = null;

        private Utils()
        {
        }

        public static Utils getInstance
        {
            get
            {
                if (__instance == null)
                {
                    __instance = new Utils();
                }

                return __instance;
            }
        }

        public string CreateTempDir()
        {
            string tempLocalPath = Path.GetTempPath();

            Guid pathGuid = Guid.NewGuid();
            string tempDir = string.Concat(tempLocalPath, @"temp_sign\", pathGuid, @"\");

            var directory = Directory.CreateDirectory(tempDir);

            return directory.FullName;
        }

        public string CreateZip(Metadata metadata, ConfigAga configAga, string pathDir)
        {
            string pathProperties = string.Concat(pathDir, Constants.PARAM_PROPERTIES);
            string pathJson = string.Concat(pathDir, Constants.JSON_METADATA);

            //create metadata - json
            string jsonMetadata = JsonConvert.SerializeObject(metadata);
            File.WriteAllText(pathJson, jsonMetadata);

            //create param - properties
            Dictionary<string, string> propertiesParam = new Dictionary<string, string>();
            propertiesParam.Add("contentFile", Constants.JSON_METADATA);
            propertiesParam.Add("timestamp", configAga.timestamp);
            propertiesParam.Add("certificateId", configAga.certificateId);
            propertiesParam.Add("secretPassword", configAga.secretPassword);

            using (StreamWriter file = new StreamWriter(pathProperties))
            {
                if (File.Exists(pathProperties))
                {
                    foreach (var entry in propertiesParam)
                    {
                        file.WriteLine("{0}={1}", entry.Key, entry.Value);
                    }
                }
            }

            try
            {
                //zip files
                SevenZipCompressor.SetLibraryPath(Path.GetFullPath("7z.dll"));
                SevenZipCompressor szc = new SevenZipCompressor();
                szc.CompressionLevel = CompressionLevel.Ultra;
                szc.CompressionMode = CompressionMode.Create;

                string sevenZOutput = string.Concat(pathDir, Constants.FILE_ZIP);
                string[] pathFiles = { pathJson, pathProperties };

                szc.CompressionMode = File.Exists(sevenZOutput) ? SevenZip.CompressionMode.Append : SevenZip.CompressionMode.Create;
                FileStream archive = new FileStream(sevenZOutput, FileMode.OpenOrCreate, FileAccess.ReadWrite);

                szc.DirectoryStructure = true;
                szc.EncryptHeaders = true;
                szc.DefaultItemName = sevenZOutput;
                szc.CompressFiles(archive, pathFiles);
                archive.Close();

                return sevenZOutput;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }

            return null;
        }

        public UriBuilder ConvertToUriBuilder(SearchRequest searchRequest, string uri)
        {
            try
            {
                UriBuilder uriBuilder = new UriBuilder(uri);

                var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                if (searchRequest.page.ToString() != null) query["page"] = searchRequest.page.ToString();
                if (searchRequest.count.ToString() != null) query["count"] = searchRequest.count.ToString();
                if (!String.IsNullOrEmpty(searchRequest.sort)) query["sort"] = searchRequest.sort;
                if (!String.IsNullOrEmpty(searchRequest.name)) query["name"] = searchRequest.name;
                if (!String.IsNullOrEmpty(searchRequest.doc)) query["doc"] = searchRequest.doc;
                if (!String.IsNullOrEmpty(searchRequest.subject)) query["subject"] = searchRequest.subject;
                if (!String.IsNullOrEmpty(searchRequest.tag)) query["tag"] = searchRequest.tag;
                if (searchRequest.status.ToString() != null) query["status"] = searchRequest.status.ToString();
                if (searchRequest.dateBegin.ToString() != "0") query["dateBegin"] = searchRequest.dateBegin.ToString();
                if (searchRequest.dateEnd.ToString() != "0") query["dateEnd"] = searchRequest.dateEnd.ToString();
                if (searchRequest.failed.ToString() != null) query["failed"] = searchRequest.failed.ToString();
                uriBuilder.Query = query.ToString();

                return uriBuilder;
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