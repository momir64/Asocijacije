using System;
using Network;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace Asocijacije {
    public partial class AsocijacijeForm : PozadinaForm {
        bool naRedu;
        readonly bool prvi;
        readonly Stinto chat;
        readonly string[][][] asocijacije;
        public AsocijacijeForm(string[][][] asocijacije, Stinto chat, bool prvi, int takmicar1, int takmicar2) : base(takmicar1, takmicar2) {
            this.asocijacije = asocijacije;
            this.chat = chat;
            this.prvi = prvi;
            naRedu = prvi;
            InitializeComponent();
        }

        void UpdateNaRedu() {
            if (prvi == naRedu)
                naReduBox.Open((naRedu ? "ВИ СТЕ" : "ПРОТИВНИК ЈЕ") + " НА ПОТЕЗУ", TextBoxRounded.Plava);
            else
                naReduBox.Open((naRedu ? "ВИ СТЕ" : "ПРОТИВНИК ЈЕ") + " НА ПОТЕЗУ", TextBoxRounded.Crvena);
        }

        bool loaded = false;
        TimerBox timerBox;
        TextBoxRounded dalje, naReduBox;
        readonly TextBoxRounded[][] kolone = new TextBoxRounded[5][];
        private void AsocijacijeForm_Load(object sender, EventArgs e) {
            PlaceScores();
            timerBox = new TimerBox(626, 63, 27, 214, this);
            Controls.Add(timerBox);
            for (int i = 0; i < 4; i++)
                kolone[i] = new TextBoxRounded[5];
            kolone[4] = new TextBoxRounded[1] { new TextBoxRounded(411f, 335f, 458f, 50f, "???", this) };
            for (int i = 0; i < 5; i++) {
                kolone[0][i] = new TextBoxRounded(409.0 - 52.2 * (i + 1), 334.8 - 54.2 * (i + 1), 279f, 50f, "А" + (5 - i), this);
                kolone[1][i] = new TextBoxRounded(590.7 + 52.5 * (i + 1), 334.8 - 54.2 * (i + 1), 279f, 50f, "Б" + (5 - i), this);
                kolone[2][i] = new TextBoxRounded(409.9 - 52.5 * (i + 1), 54.5 * (i + 1) + 334.7, 279f, 50f, "В" + (5 - i), this);
                kolone[3][i] = new TextBoxRounded(52.5 * (i + 1) + 590.7, 54.5 * (i + 1) + 334.7, 279f, 50f, "Г" + (5 - i), this);
                kolone[i][0].ResultEvent += OnResult;
            }
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 5; j++) {
                    kolone[i][j].Click += new EventHandler(TextBoxClick);
                    Controls.Add(kolone[i][j]);
                }
            naReduBox = new TextBoxRounded(440f, -8f, 400f, 50f, (naRedu ? "ВИ СТЕ" : "ПРОТИВНИК ЈЕ") + " НА ПОТЕЗУ", this);
            dalje = new TextBoxRounded(565f, 658f, 150f, 50f, "ДАЉЕ", this);
            kolone[4][0].Click += new EventHandler(TextBoxClick);
            dalje.MouseDown += new MouseEventHandler(DaljeDown);
            naReduBox.Click += new EventHandler(TextBoxClick);
            dalje.Click += new EventHandler(TextBoxClick);
            Controls.Add(kolone[4][0]);
            Controls.Add(naReduBox);
            timer.Enabled = true;
            Controls.Add(dalje);
            Tick(sender, e);
            UpdateNaRedu();
            loaded = true;
            chat.MessageEvent += OnMessageReceived;
        }

        void DaljeDown(object sender, MouseEventArgs e) {
            if (naRedu && !finished && !otvaranje)
                dalje.Color = TextBoxRounded.NeutralnaDown;
        }

        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            if (loaded) {
                for (int i = 0; i < 4; i++)
                    for (int j = 0; j < 5; j++)
                        kolone[i][j].UpdateDimensions(this);
                kolone[4][0].UpdateDimensions(this);
                naReduBox.UpdateDimensions(this);
                timerBox.UpdateDimensions(this);
                dalje.UpdateDimensions(this);
                PlaceScores();
            }
        }

        void PlaceScores() {
            double x1 = 488.0 / 1280 * Width;
            double x2 = 665.0 / 1280 * Width;
            double y = 43.0 / 720 * Height;
            double width = 133.0 / 1280 * Width;
            double height = 97.0 / 720 * Height;
            scoreL.Location = new Point((int)Math.Round(x1), (int)Math.Round(y));
            scoreL.Size = new Size((int)Math.Round(width), (int)Math.Round(height));
            scoreL.Font = new Font(scoreL.Font.FontFamily, (float)(height * 0.45), FontStyle.Bold);
            scoreD.Location = new Point((int)Math.Round(x2), (int)Math.Round(y));
            scoreD.Size = new Size((int)Math.Round(width), (int)Math.Round(height));
            scoreD.Font = new Font(scoreD.Font.FontFamily, (float)(height * 0.45), FontStyle.Bold);
        }

        float sec = 1;
        readonly int maxTime = 12000;
        private void Tick(object sender, EventArgs e) {
            if (!finished && sec <= maxTime) {
                timerBox.SetPercentage(sec / maxTime);
                sec++;
            }
            else {
                timer.Enabled = false;
                finished = true;
                RestoreTitles();
                kolone[4][0].Text = asocijacije[4][0][0];
                OnResult(kolone[4][0], false);
            }
        }

        private readonly static string[] lat = { "ДЖ", "НЈ", "ЛЈ" };
        private readonly static string[] cir = { "Џ", "Њ", "Љ" };
        private static string IspraviCir(string text) {
            text = text.ToUpper();
            for (int i = 0; i < lat.Length; i++)
                text = text.Replace(lat[i], cir[i]);
            return text;
        }

        void OnMessageReceived(string message) {
            if (message == "next") {
                dalje.Color = TextBoxRounded.NeutralnaDown;
                Task.Delay(500).Wait();
                dalje.Color = TextBoxRounded.Neutralna;
                otvaranje = true;
                naRedu = true;
                UpdateNaRedu();
                return;
            }
            string[] parts = message.Split(':');
            if (parts[0] == "open") {
                int K = Convert.ToInt32(parts[1]);
                int B = Convert.ToInt32(parts[2]);
                kolone[K][4 - B].Text = asocijacije[K][B][0];
                kolone[K][4 - B].Opened = true;
            }
            else if (parts[0] == "result") {
                RestoreTitles();
                kolone[Convert.ToInt32(parts[1])][0].Text = parts[2];
                OnResult(kolone[Convert.ToInt32(parts[1])][0]);
                otvaranje = true;
                naRedu = true;
                UpdateNaRedu();
            }
        }

        bool probano = false;
        bool finished = false;
        bool otvaranje = true;
        async void OnResult(TextBoxRounded textBox, bool addScore = true) {
            if (naRedu) await chat.SendMessageAsync("result:" + textBox.K + ":" + textBox.Text);
            Task.Delay(750).Wait();
            probano = true;
            otvaranje = true;
            foreach (string resenje in asocijacije[textBox.K][textBox.K == 4 ? 0 : 4]) {
                if (IspraviCir(textBox.Text) == resenje) {
                    int score = 0;
                    for (int i = textBox.K == 4 ? 0 : textBox.K; i < (textBox.K == 4 ? 4 : textBox.K + 1); i++) {
                        probajKonacno = true;
                        if (!kolone[i][0].Opened)
                            score += 3;
                        for (int j = 0; j < 5; j++) {
                            if (j < 4 && !kolone[i][4 - j].Opened)
                                score++;
                            if (!kolone[i][0].Opened)
                                kolone[i][4 - j].Open(asocijacije[i][j][0], prvi == naRedu ? TextBoxRounded.Plava : TextBoxRounded.Crvena);
                        }
                    }
                    if (textBox.K == 4) {
                        kolone[4][0].Open(asocijacije[4][0][0], prvi == naRedu ? TextBoxRounded.Plava : TextBoxRounded.Crvena);
                        finished = true;
                        score += 7;
                    }
                    if (addScore)
                        AddScore(scoreL, score);
                    textBox.Opened = true;
                    return;
                }
            }
            RestoreTitles();
            naRedu = false;
        }

        TextBoxRounded textBoxOld;
        bool probajKonacno = false;
        async void TextBoxClick(object sender, EventArgs e) {
            if (naRedu) {
                TextBoxRounded textBox = (TextBoxRounded)sender;
                if (!finished && otvaranje && textBox.B < 4 && !textBox.Opened) {
                    RestoreTitles();
                    await chat.SendMessageAsync("open:" + textBox.K + ":" + textBox.B);
                    otvaranje = false;
                    textBox.Text = asocijacije[textBox.K][textBox.B][0];
                    textBox.Opened = true;
                }
                else if (!finished && (!otvaranje || (probajKonacno && textBox.K == 4) || SveOtvoreno()) && textBox.B == 4 && (probano || DifferentTextBox(textBox)) && !textBox.Opened && (OtvorenaKolona(textBox.K) || textBox.B == 4)) {
                    RestoreTitles();
                    probano = false;
                    backClick = false;
                    textBox.Enabled = true;
                    textBox.ResetText();
                }
                else if (!finished && !otvaranje && textBox.B == 5) {
                    RestoreTitles();
                    await chat.SendMessageAsync("next");
                    otvaranje = true;
                    naRedu = false;
                    Task.Delay(500).Wait();
                    dalje.Color = TextBoxRounded.Neutralna;
                }
                else if (!finished && DifferentTextBox(textBox))
                    RestoreTitles();
                textBoxOld = textBox;
            }
        }

        bool SveOtvoreno() {
            for (int i = 0; i < 4; i++)
                for (int j = 1; j < 5; j++) {
                    if (!kolone[i][j].Opened)
                        return false;
                }
            return true;
        }

        bool OtvorenaKolona(int k) {
            return kolone[k].Any(x => x.Opened);
        }

        void AddScore(Label label, int score) {
            label.Text = (int.Parse(label.Text) + score).ToString();
        }

        bool DifferentTextBox(TextBoxRounded textBox) {
            return textBoxOld == null || textBox.K != textBoxOld.K || textBox.B != textBoxOld.B || backClick;
        }

        void RestoreTitles() {
            for (int i = 0; i < 5; i++)
                kolone[i][0].RestoreTitle();
        }

        bool backClick = false;
        protected override void WndProc(ref Message m) {
            base.WndProc(ref m);
            const int WM_NCLBUTTONDOWN = 0x00A1;

            if (m.Msg == WM_NCLBUTTONDOWN) {
                RestoreTitles();
                backClick = true;
            }
        }
    }
}
