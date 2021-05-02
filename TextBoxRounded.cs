using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Asocijacije {
    public partial class TextBoxRounded : UserControl {
        [DllImport("gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nheightRect, int nweightRect);

        public TextBoxRounded(double x, double y, double width, double height, string title, Form parent) {
            DoubleBuffered = true;
            InitializeComponent();
            SetDimensions(x, y, width, height, parent);
            label.Text = title[1] == '5' ? title[0].ToString() : title;
        }

        public TextBoxRounded() {
            InitializeComponent();
        }

        private void TextBoxRounded_Load(object sender, EventArgs e) {
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
            label.Location = new Point(0, 0);
            label.Width = Width;
            label.Height = (int)Math.Round(Height * 0.95);
            label.Font = new Font(label.Font.FontFamily, Height * 0.45f, FontStyle.Bold);
            textBox.Location = new Point(Width / 2, Height / 2);
            MakeBackground();
        }

        private void label_Click(object sender, EventArgs e) {
            textBox.Focus();
        }

        private void textBox_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == (char)8 && label.Text.Length > 0)
                label.Text = label.Text.Substring(0, label.Text.Length - 1);
            else if (e.KeyChar != (char)8 && label.Text.Length < 30)
                label.Text += e.KeyChar;
            while (label.Width > TextRenderer.MeasureText(label.Text, new Font(label.Font.FontFamily, label.Font.Size, label.Font.Style)).Width && label.Font.Size < Height * 0.45f)
                label.Font = new Font(label.Font.FontFamily, label.Font.Size + 0.5f, label.Font.Style);
            while (label.Width < TextRenderer.MeasureText(label.Text, new Font(label.Font.FontFamily, label.Font.Size, label.Font.Style)).Width)
                label.Font = new Font(label.Font.FontFamily, label.Font.Size - 0.5f, label.Font.Style);
        }

        protected override void OnPaint(PaintEventArgs e) {
            e.Graphics.DrawImage(background, 0, 0);
            base.OnPaint(e);
        }

        Bitmap background;
        private void MakeBackground() {
            if (background != null)
                background.Dispose();
            background = new Bitmap(Width, Height);
            using (Graphics grfx = Graphics.FromImage(background)) {
                float e1 = 0.07f;
                float e2 = 0.075f + e1;
                float r1 = 0.18f;
                float r2 = 0.13f;
                float r3 = 0.07f;

                RectangleF rec1 = new RectangleF(0, 0, Width, Height);
                RectangleF rec2 = new RectangleF(Height * e1, Height * e1, Width - Height * e1 * 2, Height * (1 - e1 * 2));
                RectangleF rec3 = new RectangleF(Height * e2, Height * e2, Width - Height * e2 * 2, Height * (1 - e2 * 2));
                grfx.FillRoundedRectangle(new SolidBrush(Color.FromArgb(0, 0, 32)), rec1, Height * r1);
                grfx.FillRoundedRectangle(new SolidBrush(Color.FromArgb(175, 176, 184)), rec2, Height * r2);
                grfx.FillRoundedRectangle(new SolidBrush(Color.FromArgb(0, 32, 105)), rec3, Height * r3);
            }
        }
    }

    public static class Extensions {
        public static void FillRoundedRectangle(this Graphics graphics, Brush brush, RectangleF bounds, float cornerRadius) {
            if (graphics == null)
                throw new ArgumentNullException("graphics");
            if (brush == null)
                throw new ArgumentNullException("brush");

            using (GraphicsPath path = RoundedRect(bounds, cornerRadius)) {
                graphics.FillPath(brush, path);
            }
        }

        private static GraphicsPath RoundedRect(RectangleF rect, float radius) {
            PointF point1, point2;
            GraphicsPath path = new GraphicsPath();
            path.AddArc(new RectangleF(rect.X, rect.Y, 2 * radius, 2 * radius), 180, 90);
            point1 = new PointF(rect.X + radius, rect.Y);
            point2 = new PointF(rect.Right - radius, rect.Y);
            path.AddLine(point1, point2);
            path.AddArc(new RectangleF(rect.Right - 2 * radius, rect.Y, 2 * radius, 2 * radius), 270, 90);
            point1 = new PointF(rect.Right, rect.Y + radius);
            point2 = new PointF(rect.Right, rect.Bottom - radius);
            path.AddLine(point1, point2);
            path.AddArc(new RectangleF(rect.Right - 2 * radius, rect.Bottom - 2 * radius, 2 * radius, 2 * radius), 0, 90);
            point1 = new PointF(rect.Right - radius, rect.Bottom);
            point2 = new PointF(rect.X + radius, rect.Bottom);
            path.AddLine(point1, point2);
            path.AddArc(new RectangleF(rect.X, rect.Bottom - 2 * radius, 2 * radius, 2 * radius), 90, 90);
            point1 = new PointF(rect.X, rect.Bottom - radius);
            point2 = new PointF(rect.X, rect.Y + radius);
            path.AddLine(point1, point2);
            path.CloseFigure();
            return path;
        }

        //public static GraphicsPath RoundedRect(Rectangle bounds, int radius) {
        //    int diameter = radius * 2;
        //    Size size = new Size(diameter, diameter);
        //    Rectangle arc = new Rectangle(bounds.Location, size);
        //    GraphicsPath path = new GraphicsPath();
        //    if (radius == 0) {
        //        path.AddRectangle(bounds);
        //        return path;
        //    }
        //    path.AddArc(arc, 180, 90);
        //    arc.X = bounds.Right - diameter;
        //    path.AddArc(arc, 270, 90);
        //    arc.Y = bounds.Bottom - diameter;
        //    path.AddArc(arc, 0, 90);
        //    arc.X = bounds.Left;
        //    path.AddArc(arc, 90, 90);
        //    path.CloseFigure();
        //    return path;
        //}
    }

    public class ReadOnlyTextBox : TextBox {
        [DllImport("user32.dll")]
        static extern bool HideCaret(IntPtr hWnd);

        public ReadOnlyTextBox() {
            this.ReadOnly = true;
            this.BackColor = Color.White;
            this.GotFocus += TextBoxGotFocus;
            this.Cursor = Cursors.Arrow; // mouse cursor like in other controls
        }

        private void TextBoxGotFocus(object sender, EventArgs args) {
            HideCaret(this.Handle);
        }
    }
}
