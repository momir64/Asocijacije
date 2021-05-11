using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Asocijacije {
    public partial class PozadinaForm : MyForm {
        public PozadinaForm(int takmicar1, int takmicar2) {
            InitializeComponent();
            RandomTakmicari(takmicar1, takmicar2);
        }

        readonly Bitmap background = new Bitmap(1280, 720);
        readonly Bitmap background2 = new Bitmap(1280, 720);
        private void PozadinaForm_Load(object sender, EventArgs e) {
            titleButtonsGrayed = Color.FromArgb(30, 50, 111);
            BackBtnClick = new EventHandler(Back_Click);
            BackBtnVisible = true;
            if (Owner != null)
                if (Owner.WindowState == FormWindowState.Maximized)
                    Fullscreen_Click(sender, e);
                else {
                    Height = Owner.Height;
                    Width = Owner.Width;
                    Left = Owner.Left;
                    Top = Owner.Top;
                }
            MakeBackground();
            BackgroundImage = background;
        }

        int clickCount = 0;
        long lastClick = 0;
        bool playing = false;
        int frameCount = 0, currentFrame = 0;
        readonly Bitmap eggImg = Properties.Resources.levi4;
        protected override void WndProc(ref Message m) {
            const int WM_NCLBUTTONDOWN = 0x00A1;
            base.WndProc(ref m);
            if (m.Msg == WM_NCLBUTTONDOWN && egg && !playing) {
                long nowClick = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                if (nowClick - lastClick > 400)
                    clickCount = 0;
                else
                    clickCount++;
                if (clickCount == 3) {
                    clickCount = 0;
                    ImageAnimator.Animate(eggImg, new EventHandler(OnFrameChanged));
                    FrameDimension dimension = new FrameDimension(eggImg.FrameDimensionsList[0]);
                    frameCount = eggImg.GetFrameCount(dimension);
                    playing = true;
                }
                lastClick = nowClick;
            }
        }

        private void Back_Click(object sender, EventArgs e) {
            Owner.Show();
            if (((DobroVečeForm)Owner).fullscreen)
                ((DobroVečeForm)Owner).Fullscreen_Click(sender, e);
            Owner.Left = Left;
            Owner.Top = Top;
            ((DobroVečeForm)Owner).Center();
            if (fullscreen)
                ((DobroVečeForm)Owner).Fullscreen_Click(sender, e);
            else {
                Owner.Height = Height;
                Owner.Width = Width;
                Owner.Left = Left;
                Owner.Top = Top;
            }
            Task.Delay(20).Wait();
            Close();
        }

        private void OnFrameChanged(object o, EventArgs e) {
            if (currentFrame == frameCount) {
                ImageAnimator.StopAnimate(eggImg, new EventHandler(OnFrameChanged));
                currentFrame = 0;
                playing = false;
                frameCount = 1;
            }
            currentFrame++;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e) {
            if (playing) {
                ImageAnimator.UpdateFrames();
                e.Graphics.DrawImage(eggImg, 0, 0, Width / 2, Height);
                e.Graphics.DrawImage(background2, 0, 0, Width, Height);
            }
        }

        bool egg;
        Bitmap levi, desni;
        private void RandomTakmicari(int takmicar1, int takmicar2) {
            egg = takmicar1 == 4;
            if (levi != null) levi.Dispose();
            levi = (Bitmap)Properties.Resources.ResourceManager.GetObject("levi" + takmicar1);
            if (desni != null) desni.Dispose();
            desni = (Bitmap)Properties.Resources.ResourceManager.GetObject("desni" + takmicar2);
        }

        private void MakeBackground() {
            using (Graphics grfx = Graphics.FromImage(background)) {
                grfx.DrawImage(levi, 0, 0);
                grfx.DrawImage(desni, 640, 0);
                grfx.DrawImage(Properties.Resources.background, 0, 0);
            }
            if (egg)
                using (Graphics grfx = Graphics.FromImage(background2)) {
                    grfx.DrawImage(desni, 640, 0);
                    grfx.DrawImage(Properties.Resources.background, 0, 0);
                }
        }
    }
}
