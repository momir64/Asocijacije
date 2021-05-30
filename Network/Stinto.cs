using Flurl;
using System;
using Flurl.Http;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Network {
    public class Stinto {
        const int waitTime = 5000;
        const char delimeter = '\uFFFF';
        const string talktome = "talktome";
        const string areyoufree = "areyoufree";
        const string yeahimhere = "yeahimhere";

        static readonly Random random = new Random();
        const string chars = "abcdefghijklmnopqrstuvwxyz";
        static string EncodeUri(string str) => Uri.EscapeDataString(str);
        static string DecodeUri(string str) => Uri.UnescapeDataString(str);
        static string RandomString(int length) => new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        string GetStringBetween(string str, string a, string b) {
            int start = str.IndexOf(a) + a.Length;
            return str.Substring(start, str.Substring(start).IndexOf(b));
        }

        async Task CreateRoomAsync(Func<string, Task> RoomCreated) {
            Trace.WriteLine("Pravim sobu");
            uri = (await new Url("https://stin.to/+").GetAsync()).ResponseMessage.RequestMessage.RequestUri;
            string room = GetStringBetween(await uri.GetStringAsync(), "data-chat-id=\"", "\"");
            Trace.WriteLine("Soba je " + room);
            await RoomCreated(room);
            StintoUrl += room + "/";
        }

        async Task LogIn2RoomAsync() {
            while (true) {
                me = RandomString(6);
                IFlurlResponse response;
                try {
                    Trace.WriteLine("Hoću da se ulogujem");
                    response = await StintoUrl.AppendPathSegment("login").WithTimeout(999).PostUrlEncodedAsync(new { nick = me, termsOfUse = "true" });
                    if (response.ResponseMessage.IsSuccessStatusCode) {
                        cookies.AddOrReplace(response.Cookies[1]);
                        break;
                    }
                }
                catch (FlurlHttpException ex) when (ex.StatusCode == 429) {
                    Trace.WriteLine("Previše brzo se logujem");
                    await Task.Delay(500);
                    continue;
                }
            }
            Trace.WriteLine("Tražim svoje ime");
            string result = await StintoUrl.AppendPathSegment("poll").SetQueryParam("seq", -2).WithCookies(cookies).GetStringAsync();
            string[] lines = result.Substring(0, result.IndexOf("\n0")).Split('\n');
            lines = lines.Take(lines.Length - 1).Where(line => line.Split('\t')[6] == me && line.Split('\t')[8] == "false").ToArray();
            me = lines[0].Split('\t')[4];
        }

        async Task GetLastIndexAsync() {
            Trace.WriteLine("Tražim index poslednje poruke");
            string result = DecodeUri((await StintoUrl.AppendPathSegment("poll").SetQueryParam("seq", 0).WithCookies(cookies).GetStringAsync()).Trim());
            index = Convert.ToInt32(result.Split('\n').Last().Split('\t')[0], 16);
        }

        async Task WaitResponseAsync(CancellationToken cancelToken) {
            await GetLastIndexAsync();
            while (!cancelToken.IsCancellationRequested) {
                Trace.WriteLine("Čekam da mi se domaćin javi");
                string result = DecodeUri(await StintoUrl.AppendPathSegment("poll").SetQueryParam("seq", index).WithCookies(cookies).GetStringAsync());
                foreach (string line in result.Trim().Split('\n')) {
                    index++;
                    string[] words = line.Split('\t');
                    if (words[3] == "t" && words[4] == "private" + delimeter + yeahimhere + delimeter + me) {
                        kolega = words[2];
                        return;
                    }
                }
            }
        }

        async Task ConnectAsync() {
            Trace.WriteLine("Zovem domaćina");
            await StintoUrl.AppendPathSegment("post").WithCookies(cookies).PostUrlEncodedAsync(new { type = "TXT", text = "private" + delimeter + talktome + delimeter + me });
            var cancellationToken = new CancellationTokenSource();
            var task = WaitResponseAsync(cancellationToken.Token);
            if (await Task.WhenAny(task, Task.Delay(waitTime, cancellationToken.Token)) == task) {
                Trace.WriteLine("Javio se");
                await task;
                Trace.WriteLine("Task gotov");
                Connected = true;
                OnConnect?.Invoke(true);
            }
            else {
                cancellationToken.Cancel();
                OnConnect?.Invoke(false);
            }
        }

        public async Task SendMessageAsync(string message) {
            Trace.WriteLine("Šaljem poruku " + message);
            await StintoUrl.AppendPathSegment("post").WithCookies(cookies).PostUrlEncodedAsync(new { type = "TXT", text = "public" + delimeter + EncodeUri(message) });
        }

        async Task SendPrivateMessageAsync(string message) {
            Trace.WriteLine("Šaljem privatnu poruku " + message);
            await StintoUrl.AppendPathSegment("post").WithCookies(cookies).PostUrlEncodedAsync(new { type = "TXT", text = "private" + delimeter + message });
        }

        public event ConnectEventHandler OnConnect;
        public delegate Task DisconnectEventHandler();
        public event DisconnectEventHandler OnDisconnect;
        public delegate Task ConnectEventHandler(bool successful);

        public async Task<string> ReadMessageAsync() {
            while (true) {
                string result;
                try {
                    Trace.WriteLine("Čitam poruke");
                    result = DecodeUri(await StintoUrl.AppendPathSegment("poll").SetQueryParam("seq", index).WithCookies(cookies).GetStringAsync(cancellationTokenSource.Token));
                }
                catch (FlurlHttpException ex) when (ex.InnerException is TaskCanceledException) {
                    return "";
                }
                if (result.Length == 0) continue;
                foreach (string line in result.Trim().Split('\n')) {
                    index++;
                    string[] words = line.Split('\t');
                    if (words[2] != me && words[3] == "t") {
                        string[] message = words[4].Split(delimeter);
                        if (message[0] == "public")
                            return DecodeUri(message[1]);
                    }
                    else if (words[2] == kolega && words[3] == "x") {
                        Connected = false;
                        OnDisconnect?.Invoke();
                        return "";
                    }
                }
            }
        }

        void ConnectManager() {
            new Thread(async () => {
                while (!Connected) {
                    string result = "";
                    try {
                        Trace.WriteLine("Čekam da mi se neko javi");
                        result = DecodeUri(await StintoUrl.AppendPathSegment("poll").SetQueryParam("seq", index).WithCookies(cookies).GetStringAsync(cancellationTokenSource.Token));
                    }
                    catch (FlurlHttpException ex) when (ex.InnerException is TaskCanceledException) {
                        return;
                    }
                    if (cancellationTokenSource.IsCancellationRequested)
                        return;
                    if (result.Length == 0)
                        continue;
                    foreach (string line in result.Trim().Split('\n')) {
                        index++;
                        string[] words = line.Split('\t');
                        if (words[2] != me && words[3] == "t") {
                            string[] message = words[4].Split(delimeter);
                            if (message[0] == "private" && (message[1] == talktome || message[1] == areyoufree)) {
                                Trace.WriteLine("Odgovaram na poziv");
                                await SendPrivateMessageAsync(yeahimhere + delimeter + message[2]);
                                if (message[1] == talktome) {
                                    kolega = message[2];
                                    Connected = true;
                                    OnConnect?.Invoke(true);
                                    break;
                                }
                            }
                        }
                    }
                }
            }) { IsBackground = true }.Start();
        }

        Uri uri;
        int index = 0;
        bool Connected;
        string me, kolega;
        readonly CookieJar cookies = new CookieJar();
        string StintoUrl = "https://stin.to/api/chat/";
        readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public void Disconnect() {
            cancellationTokenSource.Cancel();
        }

        public Stinto(ConnectEventHandler OnConnect, DisconnectEventHandler OnDisconnect) {
            this.OnConnect += OnConnect;
            this.OnDisconnect += OnDisconnect;
        }

        ~Stinto() {
            cancellationTokenSource.Cancel();
        }

        public async Task<Stinto> InitializeAsync(Func<string, Task> RoomCreated) {
            await CreateRoomAsync(RoomCreated);
            Trace.WriteLine("Logujem se u sobu");
            await LogIn2RoomAsync();
            ConnectManager();
            return this;
        }

        public static Task<Stinto> CreateAsync(Func<string, Task> RoomCreated, ConnectEventHandler OnConnect, DisconnectEventHandler OnDisconnect) {
            var server = new Stinto(OnConnect, OnDisconnect);
            return server.InitializeAsync(RoomCreated);
        }

        public async Task<Stinto> InitializeAsync(string room) {
            StintoUrl += room + "/";
            Trace.WriteLine("Logujem se u sobu");
            await LogIn2RoomAsync();
            Trace.WriteLine("Povezujem se sa domaćinom");
            await ConnectAsync();
            return this;
        }

        public static Task<Stinto> CreateAsync(string room, ConnectEventHandler OnConnect, DisconnectEventHandler OnDisconnect) {
            var server = new Stinto(OnConnect, OnDisconnect);
            return server.InitializeAsync(room);
        }

        static async Task<bool> WaitPingAsync(CookieJar cookies, string StintoUrl, string me, int index, CancellationToken cancellationToken) {
            while (!cancellationToken.IsCancellationRequested) {
                Trace.WriteLine("Čekam odgovor na ping drugi put");
                string result = DecodeUri(await StintoUrl.AppendPathSegment("poll").SetQueryParam("seq", index).WithCookies(cookies).GetStringAsync());
                foreach (string line in result.Trim().Split('\n')) {
                    index++;
                    string[] words = line.Split('\t');
                    if (words[3] == "t" && words[4] == "private" + delimeter + yeahimhere + delimeter + me)
                        return true;
                }
            }
            return false;
        }

        public static async Task<bool> PingAsync(string room) {
            int index;
            string me;
            CookieJar cookies = new CookieJar();
            string StintoUrl = "https://stin.to/api/chat/" + room + "/";
            while (true) {
                me = RandomString(6);
                IFlurlResponse response;
                try {
                    Trace.WriteLine("Logujem se u sobu za ping " + room);
                    response = await StintoUrl.AppendPathSegment("login").WithTimeout(999).PostUrlEncodedAsync(new { nick = me, termsOfUse = "true" });
                    if (response.ResponseMessage.IsSuccessStatusCode) {
                        cookies.AddOrReplace(response.Cookies[1]);
                        break;
                    }
                }
                catch (FlurlHttpException ex) when (ex.StatusCode == 429) {
                    Trace.WriteLine("Previše se logujem se u sobu za ping " + room);
                    await Task.Delay(500);
                    continue;
                }
            }
            Trace.WriteLine("Gledam ko sam ja u pingu u sobi " + room);
            string result = await StintoUrl.AppendPathSegment("poll").SetQueryParam("seq", -2).WithCookies(cookies).GetStringAsync();
            string[] lines = result.Substring(0, result.IndexOf("\n0")).Split('\n');
            lines = lines.Take(lines.Length - 1).Where(line => line.Split('\t')[6] == me && line.Split('\t')[8] == "false").ToArray();
            me = lines[0].Split('\t')[4];

            Trace.WriteLine("Čitam index poslednje poruke za ping u sobi " + room);
            result = DecodeUri((await StintoUrl.AppendPathSegment("poll").SetQueryParam("seq", 0).WithCookies(cookies).GetStringAsync()).Trim());
            index = Convert.ToInt32(result.Split('\n').Last().Split('\t')[0], 16);

            Trace.WriteLine("Zovem domaćina za ping u sobi " + room);
            await StintoUrl.AppendPathSegment("post").WithCookies(cookies).PostUrlEncodedAsync(new { type = "TXT", text = "private" + delimeter + areyoufree + delimeter + me });

            var cancellationToken = new CancellationTokenSource();
            var task = WaitPingAsync(cookies, StintoUrl, me, index, cancellationToken.Token);
            Trace.WriteLine("Čekam domaćinov odgovr za ping u sobi " + room);
            if (await Task.WhenAny(task, Task.Delay(waitTime, cancellationToken.Token)) == task)
                return true;
            Trace.WriteLine("Domaćin nije odgovrio u sobi " + room);
            cancellationToken.Cancel();
            return false;
        }
    }
}
