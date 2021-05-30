using System;
using Network;
using System.Linq;
using System.Drawing;
using System.Threading;
using static Network.Gist;
using System.Windows.Forms;
using System.Threading.Tasks;
using static Asocijacije.SlagalicaTV;

namespace Asocijacije {
    public partial class DobroVečeForm : MyForm {
        public void Center() => CenterToScreen();
        public DobroVečeForm() {
            //DeleteAllFiles();
            InitializeComponent();
            BackBtnClick = new EventHandler(Back_Click);
        }

        private async void Back_Click(object sender, EventArgs e) {
            BackBtnVisible = false;
            unetoIme = false;
            OnResize(e);
            nameBox.Focus();
            //Console.WriteLine("Delete file " + MyName);
            await DeleteFileAsync(MyName);
        }

        private void DobroVečeForm_Load(object sender, EventArgs e) {
            CenterToScreen();
            playBtn.Enabled = false;
        }

        Stinto chat;
        File[] list;
        string MyName { get => nameBox.Text; }
        private async void PlayBtn_Click(object sender, EventArgs e) {
            playBtn.Enabled = false;
            ShowLoader();
            //Console.WriteLine("List files");
            list = await ListFilesAsync();
            if (list.Any(file => file.Name == MyName)) {
                File file = await ReadFileAsync(list.First(it => it.Name == MyName));
                //Console.WriteLine("Pingujem zauzeto ime " + file?.Name ?? "[null]");
                if (file != null && !await Stinto.PingAsync(file.Content)) {
                    chat?.Disconnect();
                    chat = new Stinto(ConnectedServerAsync, DisonnectedAsync);
                    //Console.WriteLine("Initialize chat");
                    await chat.InitializeAsync(RoomCreatedAsync);
                }
                else {
                    playBtn.Enabled = true;
                    HideLoader();
                    MessageBox.Show("Име је заузето!", "Асоцијације", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else {
                chat?.Disconnect();
                chat = new Stinto(ConnectedServerAsync, DisonnectedAsync);
                //Console.WriteLine("Initialize chat");
                await chat.InitializeAsync(RoomCreatedAsync);
            }
        }

        async Task RoomCreatedAsync(string room) {
            //Console.WriteLine("Brišem sve fajlove sa imenom " + MyName);
            foreach (File file in list)
                if (file.Name == MyName)
                    await DeleteFileAsync(file);
            //Console.WriteLine("Napravljen fajl " + MyName + " sa sadržajem " + room);
            await CreateFileAsync(MyName, room);
            playBtn.Enabled = true;
            HideLoader();
            listBox.Items.Clear();
            foreach (File file in list)
                if (file.Name != MyName && !listBox.Items.Contains(file.Name))
                    listBox.Items.Add(file.Name);
            BackBtnVisible = true;
            unetoIme = true;
            OnResize(new EventArgs());
            CleanList();
            UpdateList();
        }

        private async Task ConnectedServerAsync(bool successful) {
            Invoke(new MethodInvoker(delegate () {
                ShowLoader();
            }));
            //Console.WriteLine("Čekam kolegu");
            string kolega = await chat.ReadMessageAsync();
            //Console.WriteLine("Kolega je " + kolega);
            await DeleteFileAsync(kolega);
            //Console.WriteLine("Brišem kolegu " + kolega);
            int takmicar1 = Convert.ToInt32(await chat.ReadMessageAsync());
            //Console.WriteLine("Takmičar 1 je " + takmicar1);
            int takmicar2 = Convert.ToInt32(await chat.ReadMessageAsync());
            //Console.WriteLine("Takmičar 2 je " + takmicar2);
            string datum = await ParNeparAsync(true);
            //Console.WriteLine("Datum je " + datum);
            string[][][] asocijacije = ParseData(await GetData(datum));
            //Console.WriteLine("Skinuo sam slagalicu");
            Invoke(new MethodInvoker(delegate () {
                new AsocijacijeForm(asocijacije, chat, prvi, takmicar1, takmicar2, MyName, kolega).Show(this);
                Task.Delay(20).Wait();
                Hide();
                HideLoader();
            }));
        }

        private Task DisonnectedAsync() {
            MessageBox.Show("Играч је напустио игру!", "Асоцијације", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return Task.CompletedTask;
        }

        readonly float LoaderSize = 0.2f;
        void ShowLoader() {
            loader.Visible = true;
            loader.Location = Point.Round(new PointF(Width / 2 - Height * LoaderSize / 2, Height / 2 - Height * LoaderSize / 2));
            loader.Size = Size.Round(new SizeF(Height * LoaderSize, Height * LoaderSize));
        }

        void HideLoader() {
            loader.Visible = false;
        }

        bool unetoIme = false;
        private void DobroVečeForm_Resize(object sender, EventArgs e) {
            int Top = 90;
            int Left = 50;
            int Space = 10;
            int Width = 400;
            if (unetoIme) {
                nameBox.Enabled = false;
                playBtn.Visible = false;
                listBox.Visible = true;
                nameBox.Font = new Font("Segoe UI", (float)(Size.Height * 0.02), FontStyle.Bold);
                listBox.Font = new Font("Segoe UI", (float)(Size.Height * 0.02), FontStyle.Bold);

                nameBox.Top = Size.Height * Top / 720;
                nameBox.Left = Size.Width * Left / 1280;
                nameBox.Width = Size.Width * Width / 1280;
                listBox.Top = nameBox.Bottom + Space;
                listBox.Left = nameBox.Left;
                listBox.Width = nameBox.Width;
                listBox.Height = listBox.ItemHeight * 20;
            }
            else {
                listBox.Visible = false;
                nameBox.Enabled = true;
                playBtn.Visible = true;
                nameBox.Font = new Font("Segoe UI", (float)(Size.Height * 0.02), FontStyle.Bold);
                playBtn.Font = new Font("Segoe UI", (float)(Size.Height * 0.018), FontStyle.Bold);

                nameBox.Top = Size.Height * Top / 720;
                nameBox.Left = Size.Width * Left / 1280;
                nameBox.Width = Size.Width * Width / 1280;
                playBtn.Top = nameBox.Bottom + Space;
                playBtn.Left = nameBox.Left;
                playBtn.Width = nameBox.Width;
                playBtn.Height = nameBox.Height + 5;
            }
            if (loader.Visible) {
                loader.Location = Point.Round(new PointF(Size.Width / 2 - Size.Height * LoaderSize / 2, Size.Height / 2 - Size.Height * LoaderSize / 2));
                loader.Size = Size.Round(new SizeF(Size.Height * LoaderSize, Size.Height * LoaderSize));
            }
        }

        readonly int MaxLength = 30;
        private void NameBox_KeyPress(object sender, KeyPressEventArgs e) {
            int SelectionStart = nameBox.SelectionStart;
            int SelectionLength = nameBox.SelectionLength;
            if (e.KeyChar == '\r' && playBtn.Enabled) {
                PlayBtn_Click(sender, new EventArgs());
                e.Handled = true;
                nameBox.SelectionLength = 0;
            }
            else if (e.KeyChar == '\b' && MyName.Length > 0) {
                if (SelectionLength > 0) {
                    nameBox.Text = MyName.Remove(SelectionStart, SelectionLength);
                    nameBox.SelectionStart = SelectionStart;
                }
                else if (SelectionStart > 0) {
                    nameBox.Text = MyName.Remove(SelectionStart - 1, 1);
                    nameBox.SelectionStart = SelectionStart - 1;
                }
                nameBox.SelectionLength = 0;
            }
            else if ((char.IsLetter(e.KeyChar) || lat.Contains(e.KeyChar) || e.KeyChar == ' ') && MyName.Length <= MaxLength) {
                nameBox.Text = MyName.Substring(0, SelectionStart) + Lat2Cir(e.KeyChar) + MyName.Substring(SelectionStart + SelectionLength);
                nameBox.SelectionStart = SelectionStart + 1;
                nameBox.SelectionLength = 0;
            }
            else if (e.KeyChar == '\u0001') {
                nameBox.SelectionStart = 0;
                nameBox.SelectionLength = MyName.Length;
            }
            if (e.KeyChar != '\r')
                playBtn.Enabled = MyName.Length > 0;
            e.Handled = true;
        }

        private static readonly char[] lat = { 'q', 'w', 'x', 'y', 'a', 'b', 'v', 'g', 'd', 'đ', 'e', 'ž', 'z', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'r', 's', 't', 'ć', 'u', 'f', 'h', 'c', 'č', 'š', 'Q', 'W', 'X', 'Y', 'A', 'B', 'V', 'G', 'D', 'Đ', 'E', 'Ž', 'Z', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'R', 'S', 'T', 'Ć', 'U', 'F', 'H', 'C', 'Č', 'Š', '{', '}', ':', '"', '|', '[', ']', ';', '\'', '\\' };
        private static readonly char[] cir = { 'љ', 'њ', 'џ', 'и', 'а', 'б', 'в', 'г', 'д', 'ђ', 'е', 'ж', 'з', 'и', 'ј', 'к', 'л', 'м', 'н', 'о', 'п', 'р', 'с', 'т', 'ћ', 'у', 'ф', 'х', 'ц', 'ч', 'ш', 'Љ', 'Њ', 'Џ', 'И', 'А', 'Б', 'В', 'Г', 'Д', 'Ђ', 'Е', 'Ж', 'З', 'И', 'Ј', 'К', 'Л', 'М', 'Н', 'О', 'П', 'Р', 'С', 'Т', 'Ћ', 'У', 'Ф', 'Х', 'Ц', 'Ч', 'Ш', 'Ш', 'Ђ', 'Ч', 'Ћ', 'Ж', 'ш', 'ђ', 'ч', 'ћ', 'ж' };
        public char Lat2Cir(char c) {
            if (lat.Contains(c))
                return cir[Array.IndexOf(lat, c)];
            return c;
        }

        string kolega;
        private async void ListBox_MouseDoubleClick(object sender, MouseEventArgs e) {
            int index = listBox.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches) {
                kolega = listBox.Items[index].ToString();
                ShowLoader();
                chat?.Disconnect();
                chat = new Stinto(ConnectedClientAsync, DisonnectedAsync);
                //Console.WriteLine("Initialize sa kolegom " + kolega);
                await chat.InitializeAsync((await ReadFileAsync(kolega)).Content);
            }
        }

        private async Task ConnectedClientAsync(bool successful) {
            if (successful) {
                //Console.WriteLine("Delete file " + kolega);
                await DeleteFileAsync(kolega);
                int takmicar1 = random.Next(1, 7);
                int takmicar2 = random.Next(1, 7);
                //Console.WriteLine("Šaljem moje ime " + MyName);
                await chat.SendMessageAsync(MyName);
                //Console.WriteLine("Šaljem takmičara 1 " + takmicar1);
                await chat.SendMessageAsync(takmicar1.ToString());
                //Console.WriteLine("Šaljem takmičara 2 " + takmicar2);
                await chat.SendMessageAsync(takmicar2.ToString());
                //Console.WriteLine("Igram par nepar");
                string datum = await ParNeparAsync(false);
                //Console.WriteLine("Datum je " + datum);
                string[][][] asocijacije = ParseData(await GetData(datum));
                //Console.WriteLine("Skinuo sam slagalicu");
                new AsocijacijeForm(asocijacije, chat, prvi, takmicar1, takmicar2, MyName, kolega).Show(this);
                Task.Delay(20).Wait();
                Hide();
                HideLoader();
            }
            else {
                //Console.WriteLine("Delete file " + kolega);
                await DeleteFileAsync(kolega);
                SignalUpdate.Set();
                HideLoader();
                MessageBox.Show("Играч није више доступан!", "Асоцијације", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        bool prvi;
        readonly Random random = new Random();
        private async Task<string> ParNeparAsync(bool whoami) {
            string datum;
            string mojIzbor = random.Next(2).ToString();
            //Console.WriteLine("Moj izbor je " + mojIzbor);
            await chat.SendMessageAsync(mojIzbor);
            string tvojIzbor = await chat.ReadMessageAsync();
            //Console.WriteLine("Tvoj izbor je " + tvojIzbor);
            prvi = (mojIzbor == tvojIzbor) == whoami;
            if (prvi) {
                datum = GetRandomDate();
                //Console.WriteLine("Šaljem datum " + datum);
                await chat.SendMessageAsync(datum);
            }
            else {
                //Console.WriteLine("Čekam datum");
                datum = await chat.ReadMessageAsync();
            }
            return datum;
        }

        void UpdateList() {
            new Thread(async () => {
                while (true) {
                    SignalUpdate.WaitOne(1500);
                    SignalUpdate.Reset();
                    if (Visible && listBox.Visible) {
                        //Console.WriteLine("Ažuriram igrače");
                        list = await ListFilesAsync();
                        bool prisutan = false;
                        Invoke(new MethodInvoker(delegate () {
                            listBox.Items.Clear();
                            foreach (File file in list)
                                if (file.Name != MyName && !listBox.Items.Contains(file.Name))
                                    listBox.Items.Add(file.Name);
                                else if (file.Name == MyName)
                                    prisutan = true;
                        }));
                        if (!prisutan && !loader.Visible) {
                            chat?.Disconnect();
                            chat = new Stinto(ConnectedServerAsync, DisonnectedAsync);
                            //Console.WriteLine("Initialize chat");
                            await chat.InitializeAsync(RoomCreatedUpdateAsync);
                        }
                    }
                }
            }) { IsBackground = true }.Start();
        }

        async Task RoomCreatedUpdateAsync(string room) {
            //Console.WriteLine("Create file " + MyName + " sa sadržajem " + room);
            if (listBox.Visible)
                await CreateFileAsync(MyName, room);
            SignalUpdate.Set();
        }

        async Task PingAndDeleteAsync(File item) {
            if (item.Name != MyName && item.Name != kolega) {
                //Console.WriteLine("Read file " + item?.Name ?? "[null]");
                File file = await ReadFileAsync(item);
                //Console.WriteLine("Pingujem " + item?.Name ?? "[null]");
                if (file != null && !await Stinto.PingAsync(file.Content)) {
                    //Console.WriteLine("Delete file " + file?.Name ?? "[null]");
                    await DeleteFileAsync(file);
                }
            }
        }

        readonly ManualResetEvent SignalUpdate = new ManualResetEvent(false);
        void CleanList() {
            new Thread(async () => {
                while (true) {
                    if (Visible) {
                        File[] items = list.Clone() as File[];
                        Task[] tasks = new Task[items.Length];
                        for (int i = 0; i < items.Length; i++)
                            tasks[i] = PingAndDeleteAsync(items[i]);
                        await Task.WhenAll(tasks);
                    }
                    SignalUpdate.Set();
                    await Task.Delay(10000);
                }
            }) { IsBackground = true }.Start();
        }
    }
}
