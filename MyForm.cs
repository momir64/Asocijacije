using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Asocijacije {
    public partial class MyForm : Form {
        public MyForm() {
            InitializeComponent();
            DoubleBuffered = true;
            SetStyle(ControlStyles.ResizeRedraw, true);
        }

        public Color titleButtonsGrayed = Color.FromArgb(0, 7, 166);
        private void MyForm_Load(object sender, EventArgs e) {
            ClientSize = new Size(1280, 720);
        }

        public bool backBtnVisible { set { backBtn.Visible = value; } }
        public EventHandler backBtnClick { set { backBtn.Click += value; } }

        protected override void WndProc(ref Message m) {
            const uint gripSize = 5;
            const uint WM_NCLBUTTONDBLCLK = 0x00A3;
            const uint WM_NCHITTEST = 0x0084;
            const uint WM_NCPAINT = 0x0085;
            const uint WM_MOVING = 0x216;
            const uint WM_SIZING = 0x214;

            const uint HTCLIENT = 1;
            const uint HTCAPTION = 2;
            const uint HTLEFT = 10;
            const uint HTRIGHT = 11;
            const uint HTTOP = 12;
            const uint HTTOPLEFT = 13;
            const uint HTTOPRIGHT = 14;
            const uint HTBOTTOM = 15;
            const uint HTBOTTOMLEFT = 16;
            const uint HTBOTTOMRIGHT = 17;

            const uint WMSZ_LEFT = 1;
            const uint WMSZ_RIGHT = 2;
            const uint WMSZ_TOP = 3;
            const uint WMSZ_TOPLEFT = 4;
            const uint WMSZ_TOPRIGHT = 5;
            const uint WMSZ_BOTTOM = 6;
            const uint WMSZ_BOTTOMLEFT = 7;
            const uint WMSZ_BOTTOMRIGHT = 8;

            if (m.Msg == WM_NCPAINT) {
                var v = 2;
                DwmSetWindowAttribute(Handle, 2, ref v, 4);
                MARGINS margins = new MARGINS() {
                    bottomHeight = 1,
                    leftWidth = 0,
                    rightWidth = 0,
                    topHeight = 0
                };
                DwmExtendFrameIntoClientArea(Handle, ref margins);
            }
            else if (m.Msg == WM_MOVING || m.Msg == WM_SIZING) {
                int edge = m.WParam.ToInt32();
                RECT rc = (RECT)Marshal.PtrToStructure(m.LParam, typeof(RECT));
                int width = rc.Right - rc.Left;
                int height = rc.Bottom - rc.Top;
                if (edge == WMSZ_RIGHT || edge == WMSZ_BOTTOMRIGHT || edge == WMSZ_LEFT || edge == WMSZ_BOTTOMLEFT)
                    rc.Bottom = rc.Top + width * 9 / 16;
                else if (edge == WMSZ_BOTTOM || edge == WMSZ_TOP || edge == WMSZ_TOPRIGHT)
                    rc.Right = rc.Left + height * 16 / 9;
                else if (edge == WMSZ_TOPLEFT)
                    rc.Left = rc.Right - height * 16 / 9;

                Marshal.StructureToPtr(rc, m.LParam, false);
                m.Result = (IntPtr)HTCLIENT;
                return;
            }
            else if (m.Msg == WM_NCHITTEST) {
                Point pos = PointToClient(new Point(m.LParam.ToInt32()));
                if (pos.X >= ClientSize.Width - gripSize * 2 && pos.Y >= ClientSize.Height - gripSize * 2)
                    m.Result = (IntPtr)HTBOTTOMRIGHT;
                else if (pos.X >= ClientSize.Width - gripSize && pos.Y <= gripSize)
                    m.Result = (IntPtr)HTTOPRIGHT;
                else if (pos.X <= gripSize && pos.Y >= ClientSize.Height - gripSize)
                    m.Result = (IntPtr)HTBOTTOMLEFT;
                else if (pos.X <= gripSize && pos.Y <= gripSize)
                    m.Result = (IntPtr)HTTOPLEFT;
                else if (pos.X >= ClientSize.Width - gripSize)
                    m.Result = (IntPtr)HTRIGHT;
                else if (pos.Y >= ClientSize.Height - gripSize)
                    m.Result = (IntPtr)HTBOTTOM;
                else if (pos.X <= gripSize)
                    m.Result = (IntPtr)HTLEFT;
                else if (pos.Y <= gripSize)
                    m.Result = (IntPtr)HTTOP;
                else
                    m.Result = (IntPtr)HTCAPTION;
                return;
            }
            else if (m.Msg == WM_NCLBUTTONDBLCLK)
                return;
            base.WndProc(ref m);
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        private void close_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        private void close_MouseEnter(object sender, EventArgs e) {
            ((PictureBox)sender).BackColor = Color.FromArgb(217, 43, 46);
            base.OnMouseEnter(e);
        }

        private void close_MouseLeave(object sender, EventArgs e) {
            ((PictureBox)sender).BackColor = Color.FromArgb(0, 0, 0, 0);
        }

        public bool fullscreen = false;

        public void fullscreen_Click(object sender, EventArgs e) {
            fullscreen = !fullscreen;
            if (fullscreen) {
                WindowState = FormWindowState.Maximized;
                fullscreenBtn.Image = Properties.Resources.fullscreen2;
            }
            else {
                WindowState = FormWindowState.Normal;
                fullscreenBtn.Image = Properties.Resources.fullscreen1;
            }
        }

        private void fullscreen_MouseEnter(object sender, EventArgs e) {
            ((PictureBox)sender).BackColor = titleButtonsGrayed;
        }

        private void fullscreen_MouseLeave(object sender, EventArgs e) {
            ((PictureBox)sender).BackColor = Color.FromArgb(0, 0, 0, 0);
        }

        private void minimize_Click(object sender, EventArgs e) {
            WindowState = FormWindowState.Minimized;
        }

        private void minimize_MouseEnter(object sender, EventArgs e) {
            ((PictureBox)sender).BackColor = titleButtonsGrayed;
        }

        private void minimize_MouseLeave(object sender, EventArgs e) {
            ((PictureBox)sender).BackColor = Color.FromArgb(0, 0, 0, 0);
        }

        private void MyForm_Resize(object sender, EventArgs e) {
            if (WindowState == FormWindowState.Normal && fullscreen == true)
                fullscreen_Click(sender, e);
        }

        private void backBtn_MouseEnter(object sender, EventArgs e) {
            ((PictureBox)sender).BackColor = titleButtonsGrayed;
        }

        private void backBtn_MouseLeave(object sender, EventArgs e) {
            ((PictureBox)sender).BackColor = Color.FromArgb(0, 0, 0, 0);
        }

        [DllImport("dwmapi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        public struct MARGINS {
            public int leftWidth;
            public int rightWidth;
            public int topHeight;
            public int bottomHeight;
        }
    }
}
