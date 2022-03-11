using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace csharp_http_sample
{
    class Program
    {
        private static readonly string baseUrl = "https://api.speechsuper.com/";
        private static readonly string appKey = "Insert your appKey here";
        private static readonly string secretKey = "Insert your secretKey here";

        public static string HttpAPI(int httpTimeout, String audioPath, string audioFormat, int sampleRate, string coreType, Dictionary<string, object> requests)
        {
            string res = null;

            HttpWebRequest httpWebRequest = null;
            try
            {
                string param = buildParams(audioFormat, sampleRate, requests);
                string boundary = Guid.NewGuid().ToString();
                byte[] beginBoundaryBytes = Encoding.UTF8.GetBytes("--" + boundary + "\r\n");
                byte[] endBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
                byte[] newLineBytes = Encoding.UTF8.GetBytes("\r\n");
                string contentType = "multipart/form-data";
                MemoryStream memoryStream = new MemoryStream();

                string url = baseUrl + coreType;
                httpWebRequest = WebRequest.Create(url) as HttpWebRequest; // create request
                httpWebRequest.ContentType = string.Format(contentType + "; boundary={0}", boundary);
                httpWebRequest.Method = WebRequestMethods.Http.Post;
                httpWebRequest.Timeout = httpTimeout;
                httpWebRequest.Headers.Add("Request-Index:0");//Request-Index is always 0

                //text
                string formDataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n" + "{1}\r\n";

                string formItem = string.Format(formDataTemplate, "text", param);
                byte[] formItemBytes = Encoding.UTF8.GetBytes(formItem);

                memoryStream.Write(beginBoundaryBytes, 0, beginBoundaryBytes.Length);
                memoryStream.Write(formItemBytes, 0, formItemBytes.Length);

                //audio
                const string filePartHeaderTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" + "Content-Type: application/octet-stream\r\n\r\n";

                FileInfo fileInfo = new FileInfo(audioPath);
                string fileName = fileInfo.Name;

                string fileHeaderItem = string.Format(filePartHeaderTemplate, "audio", fileName);
                byte[] fileHeaderItemBytes = Encoding.UTF8.GetBytes(fileHeaderItem);

                memoryStream.Write(newLineBytes, 0, newLineBytes.Length); //newline
                memoryStream.Write(beginBoundaryBytes, 0, beginBoundaryBytes.Length);
                memoryStream.Write(fileHeaderItemBytes, 0, fileHeaderItemBytes.Length);

                int bytesRead;
                byte[] buffer = new byte[1024];
                FileStream fileStream = new FileStream(audioPath, FileMode.Open, FileAccess.Read);
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    memoryStream.Write(buffer, 0, bytesRead);
                }
                memoryStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);


                httpWebRequest.ContentLength = memoryStream.Length;

                Stream requestStream = httpWebRequest.GetRequestStream();

                memoryStream.Position = 0;
                byte[] tempBuffer = new byte[memoryStream.Length];
                memoryStream.Read(tempBuffer, 0, tempBuffer.Length);
                memoryStream.Close();

                requestStream.Write(tempBuffer, 0, tempBuffer.Length);
                requestStream.Close();

                HttpWebResponse httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse;
                if (httpWebResponse != null)
                {
                    StreamReader reader = new StreamReader(httpWebResponse.GetResponseStream(), System.Text.Encoding.GetEncoding("utf-8"));
                    res = reader.ReadToEnd();
                    httpWebResponse.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace + "\n");
            }
            finally
            {
                if (httpWebRequest != null)
                {
                    httpWebRequest.Abort();
                }
            }
            return res;
        }

        public static string buildParams(string audioFormat, int sampleRate, Dictionary<string, object> requests)
        {
            string userId = Guid.NewGuid().ToString();
            string ts = ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000).ToString();//timestamp
            string connectSigStr = appKey + ts + secretKey;
            string startSigStr = appKey + ts + userId + secretKey;
            string connectSig = sha1Hex(connectSigStr);
            string startSig = sha1Hex(startSigStr);
            JObject param = new JObject();

            //connect
            JObject connect = new JObject();
            connect.Add("cmd", "connect");
            JObject connectParam = new JObject();

            JObject connectParamSdk = new JObject();
            connectParamSdk.Add("protocol", 2);
            connectParamSdk.Add("version", 16777472);
            connectParamSdk.Add("source", 9);
            connectParam.Add("sdk", connectParamSdk);

            JObject connectParamApp = new JObject();
            connectParamApp.Add("applicationId", appKey);
            connectParamApp.Add("sig", connectSig);
            connectParamApp.Add("timestamp", ts);
            connectParam.Add("app", connectParamApp);
            connect.Add("param", connectParam);

            param.Add("connect", connect);

            //start
            JObject start = new JObject();
            start.Add("cmd", "start");
            JObject startParam = new JObject();

            JObject startParamApp = new JObject();
            startParamApp.Add("applicationId", appKey);
            startParamApp.Add("sig", startSig);
            startParamApp.Add("timestamp", ts);
            startParamApp.Add("userId", userId);
            startParam.Add("app", startParamApp);

            JObject startParamAudio = new JObject();
            startParamAudio.Add("sampleBytes", 2);
            startParamAudio.Add("channel", 1);
            startParamAudio.Add("sampleRate", sampleRate);
            startParamAudio.Add("audioType", audioFormat);
            startParam.Add("audio", startParamAudio);

            JObject startParamRequest = new JObject();
            startParamRequest = (JObject)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(requests));
            startParamRequest.Add("tokenId", Guid.NewGuid().ToString());
            startParam.Add("request", startParamRequest);

            start.Add("param", startParam);
            param.Add("start", start);

            return param.ToString();
        }

        private static string sha1Hex(string s)
        {
            try
            {
                SHA1 sha1 = new SHA1CryptoServiceProvider();
                byte[] bytesIn = Encoding.UTF8.GetBytes(s);
                byte[] bytesOut = sha1.ComputeHash(bytesIn);
                var sb = new StringBuilder();
                foreach (byte b in bytesOut)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString().ToLower();
            }
            catch (Exception ex)
            {
                throw new Exception("SHA1 encryption error：" + ex.Message);
            }
        }


        static void Main(string[] args)
        {
            int httpTimeout = 50000; //50s
            string audioPath = "supermarket.wav";
            string audioFormat = "wav";
            int sampleRate = 16000;
            string coreType = "word.eval";
            Dictionary<string, object> requests = new Dictionary<string, object>();
            requests.Add("coreType", coreType);
            requests.Add("refText", "supermarket");
            string res = HttpAPI(httpTimeout, audioPath, audioFormat, sampleRate, coreType, requests);
            Console.WriteLine("result===>" + res);
            Console.ReadLine();
        }
    }
}
