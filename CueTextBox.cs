using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Asocijacije {
    class CueTextBox : TextBox {
        [Localizable(true)]
        public string Cue {
            get => mCue;
            set { mCue = value; UpdateCue(); }
        }
        private string mCue;
        private void UpdateCue() {
            if (IsHandleCreated && mCue != null) {
                SendMessage(Handle, 0x1501, (IntPtr)1, mCue);
            }
        }
        protected override void OnHandleCreated(EventArgs e) {
            base.OnHandleCreated(e);
            UpdateCue();
        }
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, string lp);
    }
}
