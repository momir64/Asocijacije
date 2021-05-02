using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace Asocijacije {
    public partial class DobroVečeForm : MyForm {
        public void Center() => CenterToScreen();
        public DobroVečeForm() {
            InitializeComponent();
        }

        private void DobroVečeForm_Load(object sender, EventArgs e) {
            CenterToScreen();
        }

        private void playBtn_Click(object sender, EventArgs e) {
            new AsocijacijeForm().Show(this);
            Task.Delay(20).Wait();
            Hide();
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
