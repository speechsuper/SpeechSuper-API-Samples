using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace csharp_websocket_sample
{
    class Program
    {
        private static readonly string baseUrl = "wss://api.speechsuper.com/";
        private static readonly string appKey = "Insert your appKey here";
        private static readonly string secretKey = "Insert your secretKey here";


        public static async Task WebsocketAPIAsync(String audioPath, string audioType, int sampleRate, string coreType, Dictionary<string, object> requests) { 
            Dictionary<string, string> paramDic = buildParams(audioType, sampleRate, requests);
            string connectionRequest = paramDic["connect"];
            string startRequest = paramDic["start"];
            string stopRequest = paramDic["stop"];

            WebSocketclient ws = new WebSocketclient();
            ws.OnClose = () =>
            {
                Console.WriteLine("Close connection to websocket");
            };

            ws.OnError = (Exception e) =>
            {
                Console.WriteLine($"error===>{e.StackTrace}");
            };

            ws.OnMessage = (string msg) =>
            {
                Console.WriteLine($"result===>{msg}");
            };

            await ws.ConnectAsync(baseUrl + coreType);

            //Send auth
            await ws.SendAsync(Encoding.UTF8.GetBytes(connectionRequest), WebSocketMessageType.Text);

            //Send start request
            await ws.SendAsync(Encoding.UTF8.GetBytes(startRequest), WebSocketMessageType.Text);

            //Send audio
            byte[] audio = FileContent(audioPath);
            await ws.SendAsync(audio, WebSocketMessageType.Binary);

            //Send stop request
            await ws.SendAsync(Encoding.UTF8.GetBytes(stopRequest), WebSocketMessageType.Text);
        }

        private static byte[] FileContent(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    byte[] buffur = new byte[fs.Length];
                    fs.Read(buffur,0,(int)fs.Length);
                    fs.Read(buffur,0,(int)fs.Length);
                    return buffur;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private static Dictionary<string, string> buildParams(string audioType, int sampleRate, Dictionary<string, object> requests)
        {
            Dictionary<string, string> paramDic = new Dictionary<string, string>();

            string userId = Guid.NewGuid().ToString();
            string ts = ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000).ToString();//timestamp
            string connectSigStr = appKey + ts + secretKey;
            string startSigStr = appKey + ts + userId + secretKey;
            string connectSig = sha1Hex(connectSigStr);
            string startSig = sha1Hex(startSigStr);

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

            paramDic.Add("connect", connect.ToString());

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
            startParamAudio.Add("audioType", audioType);
            startParam.Add("audio", startParamAudio);

            JObject startParamRequest = new JObject();
            startParamRequest = (JObject)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(requests));
            startParamRequest.Add("tokenId", Guid.NewGuid().ToString());
            startParam.Add("request", startParamRequest);

            start.Add("param", startParam);

            paramDic.Add("start", start.ToString());

            //stop
            paramDic.Add("stop", "{\"cmd\":\"stop\"}");

            return paramDic;
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
            string audioPath = "supermarket.wav";
            string audioType = "wav";
            int sampleRate = 16000;
            string coreType = "word.eval";
            Dictionary<string, object> requests = new Dictionary<string, object>();
            requests.Add("coreType", coreType);
            requests.Add("refText", "supermarket");
            WebsocketAPIAsync(audioPath, audioType, sampleRate, coreType, requests);
            Console.ReadLine();
        }
    }
}
