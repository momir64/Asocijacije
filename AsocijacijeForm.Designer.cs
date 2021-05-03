
namespace Asocijacije {
    partial class AsocijacijeForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AsocijacijeForm));
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.scoreL = new System.Windows.Forms.Label();
            this.scoreD = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.Tick);
            // 
            // scoreL
            // 
            this.scoreL.BackColor = System.Drawing.Color.Transparent;
            this.scoreL.Font = new System.Drawing.Font("Segoe UI", 29F, System.Drawing.FontStyle.Bold);
            this.scoreL.ForeColor = System.Drawing.Color.White;
            this.scoreL.Location = new System.Drawing.Point(488, 43);
            this.scoreL.Name = "scoreL";
            this.scoreL.Size = new System.Drawing.Size(133, 97);
            this.scoreL.TabIndex = 4;
            this.scoreL.Text = "0";
            this.scoreL.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // scoreD
            // 
            this.scoreD.BackColor = System.Drawing.Color.Transparent;
            this.scoreD.Font = new System.Drawing.Font("Segoe UI", 29F, System.Drawing.FontStyle.Bold);
            this.scoreD.ForeColor = System.Drawing.Color.White;
            this.scoreD.Location = new System.Drawing.Point(665, 43);
            this.scoreD.Name = "scoreD";
            this.scoreD.Size = new System.Drawing.Size(133, 97);
            this.scoreD.TabIndex = 5;
            this.scoreD.Text = "0";
            this.scoreD.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AsocijacijeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1280, 720);
            this.Controls.Add(this.scoreD);
            this.Controls.Add(this.scoreL);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(640, 360);
            this.Name = "AsocijacijeForm";
            this.Text = "Asocijacije";
            this.Load += new System.EventHandler(this.AsocijacijeForm_Load);
            this.Controls.SetChildIndex(this.scoreL, 0);
            this.Controls.SetChildIndex(this.scoreD, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Label scoreL;
        private System.Windows.Forms.Label scoreD;
    }
}