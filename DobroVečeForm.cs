using System;
using System.Drawing;
using System.Threading.Tasks;
using static Asocijacije.SlagalicaTV;

namespace Asocijacije {
    public partial class DobroVečeForm : MyForm {
        public void Center() => CenterToScreen();
        public DobroVečeForm() {
            InitializeComponent();
        }

        private void DobroVečeForm_Load(object sender, EventArgs e) {
            CenterToScreen();
        }

        bool once = true;
        private void playBtn_Click(object sender, EventArgs e) {
            if (once) {
                once = false;
                _ = LoadAndOpen();
            }
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

        private void DobroVečeForm_Resize(object sender, EventArgs e) {
            playBtn.Top = (int)(Size.Height * 0.18);
            playBtn.Left = (int)(Size.Width * 0.05);
            playBtn.Width = (int)(Size.Width * 0.25);
            playBtn.Height = (int)(Size.Height * 0.12);
            playBtn.Font = new Font("Tahoma", (float)(playBtn.Height * 0.2), FontStyle.Bold);
        }
    }
}
