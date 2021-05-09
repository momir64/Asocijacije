using System;
using System.Net;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Network {
    public class Stinto {
        const int waitTime = 3000;
        const char delimeter = '\uFFFF';
        const string talktome = "talktome";
        const string areyoufree = "areyoufree";
        const string yeahimhere = "yeahimhere";
        const string notrightnow = "notrightnow";

        readonly Random random = new Random();
        const string chars = "abcdefghijklmnopqrstuvwxyz";
        static string EncodeUri(string str) => Uri.EscapeDataString(str);
        static string DecodeUri(string str) => Uri.UnescapeDataString(str);
        string RandomString(int length) => new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        string GetStringBetween(string str, string a, string b) {
            int start = str.IndexOf(a) + a.Length;
            return str.Substring(start, str.Substring(start).IndexOf(b));
        }

        void CreateRoom() {
            using (var request = new HttpRequestMessage(new HttpMethod("GET"), "https://stin.to/+")) {
                uri = client.SendAsync(request).Result.RequestMessage.RequestUri;
                Clipboard.SetText(uri.ToString());
            }
            using (var request = new HttpRequestMessage(new HttpMethod("GET"), uri)) {
                HttpResponseMessage response = client.SendAsync(request).Result;
                Room = GetStringBetween(response.Content.ReadAsStringAsync().Result, "data-chat-id=\"", "\"");
            }
        }

        void LogIn2Room() {
            while (true) {
                me = RandomString(6);
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://stin.to/api/chat/" + Room + "/login")) {
                    request.Content = new StringContent("nick=" + me + "&termsOfUse=true", Encoding.UTF8, "application/x-www-form-urlencoded");
                    if (client.SendAsync(request).Result.IsSuccessStatusCode) break;
                }
            }
            using (var request = new HttpRequestMessage(new HttpMethod("GET"), "https://stin.to/api/chat/" + Room + "/poll?seq=-2")) {
                HttpResponseMessage response = client.SendAsync(request).Result;
                string result = response.Content.ReadAsStringAsync().Result;
                string[] lines = result.Substring(0, result.IndexOf("\n0")).Split('\n');
                lines = lines.Take(lines.Length - 1).Where(line => line.Split('\t')[6] == me && line.Split('\t')[8] == "false").ToArray();
                me = lines[0].Split('\t')[4];
            }
        }

        void GetLastIndex() {
            using (var request = new HttpRequestMessage(new HttpMethod("GET"), "https://stin.to/api/chat/" + Room + "/poll?seq=0")) {
                HttpResponseMessage response = client.SendAsync(request).Result;
                string result = response.Content.ReadAsStringAsync().Result.Trim();
                index = Convert.ToInt32(result.Split('\n').Last().Split('\t')[0], 16);
            }
        }

        async Task WaitResponse() {
            GetLastIndex();
            while (true) {
                using (var request = new HttpRequestMessage(new HttpMethod("GET"), "https://stin.to/api/chat/" + Room + "/poll?seq=" + index)) {
                    HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                    string result = DecodeUri(await response.Content.ReadAsStringAsync());
                    foreach (string line in result.Trim().Split('\n')) {
                        index++;
                        string[] words = line.Split('\t');
                        if (words[3] == "t") {
                            string[] message = words[4].Split(delimeter);
                            if (message[0] == "private" && message[1] == yeahimhere && message[2] == me) {
                                kolega = words[2];
                                return;
                            }
                            else if (message[0] == "private" && message[1] == notrightnow && message[2] == me) {
                                kolega = null;
                                return;
                            }
                        }
                    }
                }
            }
        }

        bool Connect() {
            using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://stin.to/api/chat/" + Room + "/post")) {
                request.Content = new StringContent("type=TXT&text=private" + delimeter + talktome + delimeter + me, Encoding.UTF8, "application/x-www-form-urlencoded");
                client.SendAsync(request).Wait();
            }
            return WaitResponse().Wait(waitTime) && kolega != null;
        }

        public void SendMessage(string message) {
            using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://stin.to/api/chat/" + Room + "/post")) {
                request.Content = new StringContent("type=TXT&text=public" + delimeter + EncodeUri(message), Encoding.UTF8, "application/x-www-form-urlencoded");
                client.SendAsync(request).Wait();
            }
        }

        void SendPrivateMessage(string message) {
            using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://stin.to/api/chat/" + Room + "/post")) {
                request.Content = new StringContent("type=TXT&text=private" + delimeter + EncodeUri(message), Encoding.UTF8, "application/x-www-form-urlencoded");
                client.SendAsync(request).Wait();
            }
        }

        string message;
        bool waitMessage = false;
        public event ConnectEventHandler OnConnect;
        public delegate void ConnectEventHandler();
        public event MessageEventHandler MessageEvent;
        public delegate void MessageEventHandler(string message);
        readonly ManualResetEvent SignalConnectEvent = new ManualResetEvent(false);
        readonly ManualResetEvent SignalMessageEvent = new ManualResetEvent(false);
        readonly ManualResetEvent SignalMessageEventBack = new ManualResetEvent(false);
        readonly ManualResetEvent SignalMessageEventWait = new ManualResetEvent(false);
        void MessageReader() {
            new Task(() => {
                while (true) {
                    if (MessageEvent == null && !waitMessage) {
                        SignalMessageEventWait.WaitOne();
                        SignalMessageEventWait.Reset();
                    }
                    using (var request = new HttpRequestMessage(new HttpMethod("GET"), "https://stin.to/api/chat/" + Room + "/poll?seq=" + index)) {
                        HttpResponseMessage response = client.SendAsync(request).Result;
                        string result = DecodeUri(response.Content.ReadAsStringAsync().Result);
                        if (MessageEvent == null)
                            result = result.Trim().Split('\n')[0];
                        if (result.Length == 0)
                            continue;
                        foreach (string line in result.Trim().Split('\n')) {
                            index++;
                            string[] words = line.Split('\t');
                            if (words[2] != me) {
                                if (words[3] == "t") {
                                    string[] message = words[4].Split(delimeter);
                                    if (message[0] == "public") {
                                        MessageEvent?.Invoke(message[1]);
                                        if (waitMessage) {
                                            this.message = message[1];
                                            SignalMessageEvent.Set();
                                            SignalMessageEventBack.WaitOne();
                                            SignalMessageEventBack.Reset();
                                        }
                                    }
                                }
                                else if (words[3] == "x" && words[2] == kolega) {
                                    SignalConnectEvent.Reset();
                                    Connected = false;
                                    while (!Connected)
                                        Connected = Connect();
                                }
                            }
                        }
                    }
                }
            }).Start();
        }

        void ConnectManager() {
            new Task(() => {
                int index = 0;
                while (true) {
                    using (var request = new HttpRequestMessage(new HttpMethod("GET"), "https://stin.to/api/chat/" + Room + "/poll?seq=" + index)) {
                        HttpResponseMessage response = client.SendAsync(request).Result;
                        string result = DecodeUri(response.Content.ReadAsStringAsync().Result);
                        if (result.Length == 0)
                            continue;
                        foreach (string line in result.Trim().Split('\n')) {
                            index++;
                            string[] words = line.Split('\t');
                            if (words[2] != me && words[3] == "t") {
                                string[] message = words[4].Split(delimeter);
                                if (message[0] == "private" && (message[1] == talktome || message[1] == areyoufree)) {
                                    if (Connected)
                                        SendPrivateMessage(notrightnow + delimeter + message[2]);
                                    else {
                                        SendPrivateMessage(yeahimhere + delimeter + message[2]);
                                        if (message[1] == talktome) {
                                            kolega = message[2];
                                            Connected = true;
                                            new Task(() => { OnConnect?.Invoke(); }).Start();
                                            SignalConnectEvent.Set();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }).Start();
        }

        public string ReadMessage() {
            SignalMessageEventWait.Set();
            waitMessage = true;
            SignalMessageEvent.WaitOne();
            SignalMessageEvent.Reset();
            string message = this.message;
            waitMessage = false;
            SignalMessageEventBack.Set();
            return message;
        }

        public void Wait4Connect() {
            if (Connected) return;
            SignalConnectEvent.WaitOne();
        }

        Uri uri;
        int index;
        string me, kolega;
        readonly HttpClient client;
        readonly CookieContainer cookies;
        readonly HttpClientHandler handler;
        public string Room { get; private set; }
        public bool Connected { get; private set; }

        public Stinto() {
            cookies = new CookieContainer();
            handler = new HttpClientHandler { CookieContainer = cookies };
            client = new HttpClient(handler);
            CreateRoom();
            LogIn2Room();
            ConnectManager();
            MessageReader();
        }

        public Stinto(string room) {
            Room = room;
            cookies = new CookieContainer();
            handler = new HttpClientHandler { CookieContainer = cookies };
            client = new HttpClient(handler);
            LogIn2Room();
            Connected = Connect();
            if (Connected)
                MessageReader();
        }
    }
}
