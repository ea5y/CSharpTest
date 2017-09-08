using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Easy.CsharpTest
{
    public class BaseRes
    {

    }

    public class HttpClient
    {
        public static string AES_KEY = "i2H2m1HuUofKWArw";
        public struct Header
        {

        }

        public class TestRes : BaseRes
        {
            public string result;
            public string data;
        }

        //Test
        public void Run()
        {
            Thread thread = new Thread(new ThreadStart(this.Update));
            thread.Start();
            string devicedId = "ABXTE12345";

            var aesKey = AES_KEY;
            string data = "{\"deviceid\":\"" + devicedId + "\"}";
            string data2 = EncryptAes(data, aesKey);

            string url = @"http://192.168.1.135:8086/game/generatevisitor";
            string para = "{\"gameid\":167,data:\"" + data2 + "\"}";

            //Post<TestRes>(url, para, (res) => {
            //    Console.WriteLine("Recieve:{0}", DecryptAes(res.data, AES_KEY));
            //});

            //data = "{\"visitorname\":\"visitor_4IBXQXIQ\",\"deviceid\":\"ABXTE12345\"}";
            data = "{\"visitorname\":\"visitor_8R5EJK9I\",\"deviceid\":\"ABXTE12345\"}";
            data2 = EncryptAes(data, aesKey);
            url = @"http://192.168.1.135:8086/game/visitorlogin";
            para = "{\"gameid\":167,data:\"" + data2 + "\"}";

            Post<TestRes>(url, para, (res) => {
                Console.WriteLine("Recieve:{0}", DecryptAes(res.data, AES_KEY));
            });
        }

        private class RequestData<T> where T : BaseRes
        {
            public string Method;
            public string Url;
            public Action<T> Callback;
            public string Parameter;
        }

        private static void Get<T>(string url, Action<T> callback) where T : BaseRes
        {
            var data = new RequestData<T>() { Method = "GET", Url = url, Callback = callback };
            Send(data);
        }

        private static void Post<T>(string url, string parameter, Action<T> callback) where T : BaseRes
        {
            var data = new RequestData<T>() { Method = "POST", Url = url, Parameter = parameter, Callback = callback };
            Send(data);
        }

        private static void Send<T>(RequestData<T> data) where T : BaseRes
        {
            ThreadPool.SetMaxThreads(1000, 1000);
            ThreadPool.QueueUserWorkItem(new WaitCallback(RequestThread<T>), data);
            //RequestThread<T>(data);
        }

        private static void RequestThread<T>(object obj) where T : BaseRes
        {
            var data = obj as RequestData<T>;

            try
            {
                var request = CreateRequest(data);
                AddParameter(request, data.Parameter);
                GetRecieve(request, data);
            }
            catch (WebException we)
            {
                var msg = "RequestError: " + we.Message;
                Console.WriteLine(msg);
            }
        }

        private static HttpWebRequest CreateRequest<T>(RequestData<T> data) where T : BaseRes
        {
            var msg = string.Format("Request:\nMethod: {0} URL: {1}", data.Method, data.Url);
            Console.WriteLine(msg);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(data.Url);
            request.Method = data.Method;
            request.Timeout = 5000;
            request.ReadWriteTimeout = 5000;
            return request;
        }

        private static void AddParameter(HttpWebRequest request, string parameter)
        {
            Console.WriteLine("Parameter:{0}", parameter);
            using (Stream stream = request.GetRequestStream())
            {
                var body = Encoding.UTF8.GetBytes(parameter);
                stream.Write(body, 0, body.Length);
            }
        }

        private static void GetRecieve<T>(HttpWebRequest request, RequestData<T> data) where T : BaseRes
        {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (var responseStream = response.GetResponseStream())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        StreamReader reader = new StreamReader(responseStream);
                        var str = reader.ReadToEnd();
                        var res = JsonMapper.ToObject<T>(str);

                        var msg = string.Format("Response: {0}", str);
                        Console.WriteLine(msg);
                        InvokeAsync(() =>
                        {
                            data.Callback(res);
                                //TestStringRes(str);
                            });
                    }
                }
        }

        private static Queue<Action> _queue = new Queue<Action>();
        private void Update()
        {
            //Since in console, so i should create a new thread to recieve the res
            while(true)
            {
                if (_queue.Count > 0)
                {
                    //var msg = "InvokeAsync...";
                    Action action = _queue.Dequeue();
                    action();
                }
            }
        }

        private static void InvokeAsync(Action action)
        {
            _queue.Enqueue(action);
        }

        private static byte[] GetAesKey(string key)
        {
            if(string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key", "");
            }
            if(key.Length < 32)
            {
                key = key.PadRight(32, '0');
            }
            if(key.Length > 32)
            {
                key = key.Substring(0, 32);
            }

            return Encoding.UTF8.GetBytes(key);
        }

        private static string EncryptAes(string source, string key)
        {
            using(AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider())
            {
                aesProvider.Key = Encoding.UTF8.GetBytes(key);//GetAesKey(key);
                aesProvider.Mode = CipherMode.ECB;
                aesProvider.Padding = PaddingMode.PKCS7;
                using(ICryptoTransform cryptoTransform = aesProvider.CreateEncryptor())
                {
                    byte[] inputBuffers = Encoding.UTF8.GetBytes(source);
                    byte[] results = cryptoTransform.TransformFinalBlock(inputBuffers, 0, inputBuffers.Length);
                    aesProvider.Clear();
                    aesProvider.Dispose();
                    return Convert.ToBase64String(results, 0, results.Length);
                }
            }
        }

        private static string DecryptAes(string source, string key)
        {
            using (AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider())
            {
                aesProvider.Key = Encoding.UTF8.GetBytes(key);//GetAesKey(key);
                aesProvider.Mode = CipherMode.ECB;
                aesProvider.Padding = PaddingMode.PKCS7;
                using (ICryptoTransform cryptoTransform = aesProvider.CreateDecryptor())
                {
                    byte[] inputBuffers = Convert.FromBase64String(source);
                    byte[] results = cryptoTransform.TransformFinalBlock(inputBuffers, 0, inputBuffers.Length);
                    aesProvider.Clear();
                    return Encoding.UTF8.GetString(results);
                }
            }
        }
    }
}
