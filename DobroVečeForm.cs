﻿using System;
using Network;
using System.Linq;
using System.Drawing;
using static Network.Gist;
using System.Windows.Forms;
using System.Threading.Tasks;
using static Asocijacije.SlagalicaTV;
using System.Threading;

namespace Asocijacije {
    public partial class DobroVečeForm : MyForm {
        public void Center() => CenterToScreen();
        public DobroVečeForm() {
            InitializeComponent();
            BackBtnClick = new EventHandler(Back_Click);
        }

        private void Back_Click(object sender, EventArgs e) {
            DeleteFile(MyName);
            BackBtnVisible = false;
            unetoIme = false;
            OnResize(e);
            nameBox.Focus();
        }

        private void DobroVečeForm_Load(object sender, EventArgs e) {
            CenterToScreen();
            playBtn.Enabled = false;
        }

        bool once = true;
        //private void PlayBtn_Click(object sender, EventArgs e) {
        //    if (once) {
        //        once = false;
        //        _ = LoadAndOpen();
        //    }
        //}

        Stinto chat;
        File[] list;
        string MyName { get => nameBox.Text; }
        private void PlayBtn_Click(object sender, EventArgs e) {
            list = ListFiles();
            if (list.Any(file => file.Name == MyName)) {
                MessageBox.Show("Ime je zauzeto!");
            }
            else {
                chat = new Stinto();
                chat.OnConnect += Connected;
                CreateFile(MyName, chat.Room);
                listBox.Items.Clear();
                foreach (File file in list)
                    listBox.Items.Add(file.Name);
                UpdateList();
                CleanList();
                BackBtnVisible = true;
                unetoIme = true;
                OnResize(e);
            }
        }

        private void Connected() {
            DeleteFile(MyName);
            string kolega = chat.ReadMessage();
            KamenPapirMakaze(true);
        }

        async Task LoadAndOpen() {
            float size = 0.2f;
            loader.Visible = true;
            loader.Location = Point.Round(new PointF(Width / 2 - Height * size / 2, Height / 2 - Height * size / 2));
            loader.Size = Size.Round(new SizeF(Height * size, Height * size));
            new AsocijacijeForm(ParseData(await GetData(GetRandomDate()))).Show(this);
            Task.Delay(20).Wait();
            Hide();
            loader.Visible = false;
            once = true;
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

        private void ListBox_MouseDoubleClick(object sender, MouseEventArgs e) {
            int index = listBox.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches) {
                string kolega = listBox.Items[index].ToString();
                chat = new Stinto(ReadFile(kolega).Content);
                if (chat.Connected) {
                    DeleteFile(MyName);
                    chat.SendMessage(MyName);
                    KamenPapirMakaze(false);
                }
                else {
                    MessageBox.Show("Igrač nije više dostupan!");
                }
            }
        }

        bool prvi;
        private void KamenPapirMakaze(bool whoami) {
            string datum;
            string mojIzbor = new Random().Next(2).ToString();
            chat.SendMessage(mojIzbor);
            string tvojIzbor = chat.ReadMessage();
            prvi = (mojIzbor == tvojIzbor) == whoami;
            if (prvi) {
                datum = GetRandomDate();
                chat.SendMessage(datum);
            }
            else {
                datum = chat.ReadMessage();
            }
            MessageBox.Show(prvi + " " + datum);
        }

        void UpdateList() {
            Task.Run(() => {
                while (true) {
                    list = ListFiles();
                    Invoke(new MethodInvoker(delegate () {
                        listBox.Items.Clear();
                        foreach (File file in list)
                            if (file.Name != MyName)
                                listBox.Items.Add(file.Name);
                    }));
                    Task.Delay(2000).Wait();
                }
            });
        }

        void CleanList() {
            Task.Run(() => {
                while (true) {
                    File[] items = list.Clone() as File[];
                    Task[] tasks = new Task[items.Length];
                    for (int i = 0; i < items.Length; i++)
                        tasks[i] = new Task((object index) => {
                            if (items[(int)index].Name != MyName) {
                                File file = ReadFile(items[(int)index]);
                                if (!Stinto.Ping(file.Content))
                                    DeleteFile(file);
                            }
                        }, i);
                    foreach (Task task in tasks)
                        task.Start();
                    Task.WaitAll(tasks);
                    Task.Delay(10000).Wait();
                }
            });
        }
    }
}
