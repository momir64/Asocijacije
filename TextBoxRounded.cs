using System;
using System.Linq;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Asocijacije {
    public partial class TextBoxRounded : UserControl {
        [DllImport("gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nheightRect, int nweightRect);
        readonly public static Color Plava = Color.FromArgb(32, 160, 254);
        readonly public static Color Crvena = Color.FromArgb(205, 0, 1);
        readonly public static Color Neutralna = Color.FromArgb(0, 32, 105);

        public int K;
        public int B;
        public string Title;
        public TextBoxRounded(double x, double y, double width, double height, string title, Form parent) {
            DoubleBuffered = true;
            InitializeComponent();
            SetDimensions(x, y, width, height, parent);
            label.Text = title[1] == '5' ? title[0].ToString() : title;
            Title = title;
            K = title[0] == 'А' ? 0 : title[0] == 'Б' ? 1 : title[0] == 'В' ? 2 : title[0] == 'Г' ? 3 : title[0] == '?' ? 4 : 5;
            B = (int)char.GetNumericValue(title[1]) - 1;
            B = B == -2 ? 4 : B;
            B = K == 5 ? 5 : B;
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

        private float FontSize = 0.4f;
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
            label.Font = new Font(label.Font.FontFamily, Height * FontSize, FontStyle.Bold);
            textBox.Location = new Point(Width / 2, Height / 2);
            MakeBackground();
        }

        public bool Opened = false;
        public void RestoreTitle() {
            if (!Opened)
                label.Text = Title[1] == '5' ? Title[0].ToString() : Title;
            label.Focus();
        }

        Color InsideColor = Neutralna;
        public void Open(string str, Color color) {
            Opened = true;
            InsideColor = color;
            MakeBackground();
            Invalidate();
            label.Text = str;
        }

        public event ResultEventHandler ResultEvent;
        public delegate void ResultEventHandler(TextBoxRounded textBox, bool addScore = true);
        private void textBox_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == '\r' && label.Text.Length > 0) {
                label.Focus();
                ResultEvent(this);
            }
            else if (e.KeyChar == (char)8 && label.Text.Length > 0)
                label.Text = label.Text.Substring(0, label.Text.Length - 1);
            else if (char.IsLetter(e.KeyChar) && label.Text.Length < 30)
                label.Text += Lat2Cir(e.KeyChar);
            textBox.Text = "";
            e.Handled = true;
        }

        private static char[] lat = { 'Q', 'W', 'X', 'Y', 'A', 'B', 'V', 'G', 'D', 'Đ', 'E', 'Ž', 'Z', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'R', 'S', 'T', 'Ć', 'U', 'F', 'H', 'C', 'Č', 'Š' };
        private static char[] cir = { 'Љ', 'Њ', 'Џ', 'И', 'А', 'Б', 'В', 'Г', 'Д', 'Ђ', 'Е', 'Ж', 'З', 'И', 'Ј', 'К', 'Л', 'М', 'Н', 'О', 'П', 'Р', 'С', 'Т', 'Ћ', 'У', 'Ф', 'Х', 'Ц', 'Ч', 'Ш' };
        public char Lat2Cir(char c) {
            c = char.ToUpper(c);
            if (lat.Contains(c))
                return cir[Array.IndexOf(lat, c)];
            return c;
        }

        private void label_TextChanged(object sender, EventArgs e) {
            while (label.Width * 0.9 > TextRenderer.MeasureText(label.Text, new Font(label.Font.FontFamily, label.Font.Size, label.Font.Style)).Width && label.Font.Size < Height * FontSize)
                label.Font = new Font(label.Font.FontFamily, label.Font.Size + 0.5f, label.Font.Style);
            while (label.Width * 0.9 < TextRenderer.MeasureText(label.Text, new Font(label.Font.FontFamily, label.Font.Size, label.Font.Style)).Width)
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
                grfx.FillRoundedRectangle(new SolidBrush(InsideColor), rec3, Height * r3);
            }
        }

        private void onClick(object sender, EventArgs e) => base.OnClick(e);
        public override string Text {
            set => label.Text = value;
            get => label.Text;
        }
        public new bool Enabled {
            set { textBox.Enabled = value; textBox.Focus(); }
            get => textBox.Enabled;
        }
    }
}
