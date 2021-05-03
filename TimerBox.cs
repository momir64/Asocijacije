using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Asocijacije {
    public partial class TimerBox : UserControl {
        [DllImport("gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nheightRect, int nweightRect);

        public TimerBox(double x, double y, double width, double height, Form parent) {
            Name = "TimerBox";
            Load += new EventHandler(TimerBox_Load);
            DoubleBuffered = true;
            SetDimensions(x, y, width, height, parent);
        }

        private void TimerBox_Load(object sender, EventArgs e) {
            BackColor = Color.Transparent;
        }

        double x, y, width, height;
        private void SetDimensions(double x, double y, double width, double height, Form parent) {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            UpdateDimensions(parent);
        }

        public void UpdateDimensions(Form parent) {
            double x = this.x / 1280 * parent.Width;
            double y = this.y / 720 * parent.Height;
            double width = this.width / 1280 * parent.Width;
            double height = this.height / 720 * parent.Height;
            Location = new Point((int)Math.Round(x), (int)Math.Round(y));
            Size = new Size((int)Math.Round(width), (int)Math.Round(height));
            MakeBackground();
        }

        float percent = 0;
        public void SetPercentage(float p) {
            percent = Math.Min(1, p);
            MakeBackground();
        }

        protected override void OnPaint(PaintEventArgs e) {
            e.Graphics.DrawImage(background, 0, 0);
            base.OnPaint(e);
        }

        protected override void OnResize(EventArgs e) {
            MakeBackground();
            base.OnResize(e);
        }

        Bitmap background;
        private void MakeBackground() {
            if (background != null)
                background.Dispose();
            background = new Bitmap(Width, Height);
            using (Graphics grfx = Graphics.FromImage(background)) {
                float e1 = 0.115f;
                float e2 = 0.135f + e1;
                float r1 = 0.15f;
                float r2 = 0.13f;

                RectangleF rec1 = new RectangleF(0, 0, Width, Height);
                RectangleF rec2 = new RectangleF(Width * e1, Width * e1, Width * (1 - e1 * 2), Height - Width * e1 * 2);
                RectangleF rec3 = new RectangleF(Width * e2, Width * e2, Width * (1 - e2 * 2), Height - Width * e2 * 2);
                RectangleF rec4 = new RectangleF(Width * e2, Width * e2, Width * (1 - e2 * 2), (Height - Width * e2 * 2) * percent);
                grfx.FillRoundedRectangle(new SolidBrush(Color.FromArgb(0, 22, 85)), rec1, Width * r1);
                grfx.FillRoundedRectangle(new SolidBrush(Color.FromArgb(165, 169, 179)), rec2, Width * r2);
                grfx.FillRectangle(new SolidBrush(Color.Black), rec3);
                grfx.FillRectangle(new SolidBrush(Color.White), rec4);
            }
            Invalidate();
        }
    }
}
