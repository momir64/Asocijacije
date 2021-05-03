using System;
using System.Drawing;
using System.Windows.Forms;

//neotkriveno 1
//kolona 3
//konačno 7
namespace Asocijacije {
    public partial class AsocijacijeForm : PozadinaForm {
        string[][][] asocijacije;
        public AsocijacijeForm(string[][][] asocijacije) {
            this.asocijacije = asocijacije;
            InitializeComponent();
        }

        bool loaded = false;
        TimerBox timerBox;
        TextBoxRounded dalje;
        TextBoxRounded[][] kolone = new TextBoxRounded[5][];
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
                kolone[i][0].ResultEvent += Result;
            }
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 5; j++) {
                    kolone[i][j].Click += new EventHandler(TextBoxClick);
                    Controls.Add(kolone[i][j]);
                }
            dalje = new TextBoxRounded(565f, 658f, 150f, 50f, "ДАЉЕ", this);
            kolone[4][0].Click += new EventHandler(TextBoxClick);
            dalje.Click += new EventHandler(TextBoxClick);
            Controls.Add(kolone[4][0]);
            Controls.Add(dalje);
            loaded = true;
        }

        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            if (loaded) {
                for (int i = 0; i < 4; i++)
                    for (int j = 0; j < 5; j++)
                        kolone[i][j].UpdateDimensions(this);
                kolone[4][0].UpdateDimensions(this);
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
        bool started = false;
        private void timer_Tick(object sender, EventArgs e) {
            timerBox.SetPercentage(sec / 120);
            sec++;
        }

        bool otvaranje = true;
        TextBoxRounded textBoxOld;
        void TextBoxClick(object sender, EventArgs e) {
            TextBoxRounded textBox = (TextBoxRounded)sender;
            if (!started) {
                started = true;
                timer.Enabled = true;
                timer_Tick(sender, e);
            }

            if (otvaranje && textBox.B < 4) {
                otvaranje = false;
                textBox.Text = asocijacije[textBox.K][textBox.B][0];
            }
            else if (otvaranje == false && textBox.B == 4 && differentTextBox(textBox)) {
                RestoreTitles();
                backClick = false;
                textBox.Enabled = true;
                textBox.ResetText();
            }
            else if (otvaranje == false && textBox.B == 5) {
                RestoreTitles();
                otvaranje = true;
            }

            textBoxOld = textBox;
        }

        void Result(TextBoxRounded textBox) {
            otvaranje = true;
            foreach (string resenje in asocijacije[textBox.K][4]) {
                if (textBox.Text == resenje) {
                    textBox.Otvoreno = true;
                    return;
                }
            }
            RestoreTitles();
        }

        bool differentTextBox(TextBoxRounded textBox) {
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
