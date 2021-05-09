
namespace Asocijacije {
    partial class DobroVečeForm {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DobroVečeForm));
            this.playBtn = new System.Windows.Forms.Button();
            this.loader = new System.Windows.Forms.PictureBox();
            this.nameBox = new Asocijacije.CueTextBox();
            this.listBox = new System.Windows.Forms.ListBox();
            this.UpdateListTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.loader)).BeginInit();
            this.SuspendLayout();
            // 
            // playBtn
            // 
            this.playBtn.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.playBtn.Location = new System.Drawing.Point(45, 153);
            this.playBtn.Name = "playBtn";
            this.playBtn.Size = new System.Drawing.Size(400, 45);
            this.playBtn.TabIndex = 1;
            this.playBtn.Text = "ИГРАЈ";
            this.playBtn.UseVisualStyleBackColor = true;
            this.playBtn.Click += new System.EventHandler(this.PlayBtn_Click);
            // 
            // loader
            // 
            this.loader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.loader.Image = ((System.Drawing.Image)(resources.GetObject("loader.Image")));
            this.loader.Location = new System.Drawing.Point(1218, 658);
            this.loader.Name = "loader";
            this.loader.Size = new System.Drawing.Size(50, 50);
            this.loader.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.loader.TabIndex = 4;
            this.loader.TabStop = false;
            this.loader.Visible = false;
            // 
            // nameBox
            // 
            this.nameBox.BackColor = System.Drawing.SystemColors.Window;
            this.nameBox.Cue = "Име";
            this.nameBox.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.nameBox.Location = new System.Drawing.Point(45, 90);
            this.nameBox.Name = "nameBox";
            this.nameBox.Size = new System.Drawing.Size(400, 45);
            this.nameBox.TabIndex = 7;
            this.nameBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.NameBox_KeyPress);
            // 
            // listBox
            // 
            this.listBox.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.listBox.FormattingEnabled = true;
            this.listBox.ItemHeight = 38;
            this.listBox.Location = new System.Drawing.Point(45, 155);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(400, 460);
            this.listBox.TabIndex = 3;
            this.listBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ListBox_MouseDoubleClick);
            // 
            // UpdateList
            // 
            this.UpdateListTimer.Enabled = true;
            this.UpdateListTimer.Interval = 1000;
            this.UpdateListTimer.Tick += new System.EventHandler(this.UpdateList);
            // 
            // DobroVečeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::Asocijacije.Properties.Resources.dobroveče;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1280, 720);
            this.Controls.Add(this.nameBox);
            this.Controls.Add(this.listBox);
            this.Controls.Add(this.loader);
            this.Controls.Add(this.playBtn);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(640, 360);
            this.Name = "DobroVečeForm";
            this.Load += new System.EventHandler(this.DobroVečeForm_Load);
            this.Resize += new System.EventHandler(this.DobroVečeForm_Resize);
            this.Controls.SetChildIndex(this.playBtn, 0);
            this.Controls.SetChildIndex(this.loader, 0);
            this.Controls.SetChildIndex(this.listBox, 0);
            this.Controls.SetChildIndex(this.nameBox, 0);
            ((System.ComponentModel.ISupportInitialize)(this.loader)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button playBtn;
        private System.Windows.Forms.PictureBox loader;
        private CueTextBox nameBox;
        private System.Windows.Forms.ListBox listBox;
        private System.Windows.Forms.Timer UpdateListTimer;
    }
}