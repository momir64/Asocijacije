using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Asocijacije {
    public partial class AsocijacijeForm : PozadinaForm {
        public AsocijacijeForm() {
            InitializeComponent();
        }

        bool loaded = false;
        TextBoxRounded[][] kolone = new TextBoxRounded[5][];
        private void AsocijacijeForm_Load(object sender, EventArgs e) {
            for (int i = 0; i < 4; i++)
                kolone[i] = new TextBoxRounded[5];
            for (int i = 1; i <= 5; i++) {
                kolone[0][i - 1] = new TextBoxRounded(409 - 52.2 * i, 334.8 - 54.2 * i, 279f, 50f, "А" + (6 - i), this);
                kolone[1][i - 1] = new TextBoxRounded(590.7 + 52.5 * i, 334.8 - 54.2 * i, 279f, 50f, "Б" + (6 - i), this);
                kolone[2][i - 1] = new TextBoxRounded(409.9 - 52.5 * i, 54.5 * i + 334.7, 279f, 50f, "В" + (6 - i), this);
                kolone[3][i - 1] = new TextBoxRounded(52.5 * i + 590.7, 54.5 * i + 334.7, 279f, 50f, "Г" + (6 - i), this);
                for (int k = 0; k < 4; k++)
                    for (int j = 0; j < 5; j++)
                        Controls.Add(kolone[k][j]);
            }
            kolone[4] = new TextBoxRounded[1] { new TextBoxRounded(411f, 335f, 458f, 50f, "???", this) };
            Controls.Add(kolone[4][0]);
            loaded = true;
        }

        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            if (loaded) {
                for (int i = 0; i < 4; i++)
                    for (int j = 0; j < 5; j++)
                        kolone[i][j].UpdateDimensions(this);
                kolone[4][0].UpdateDimensions(this);
            }
        }
    }
}
