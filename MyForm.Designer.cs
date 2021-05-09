using System;
using System.Drawing;
using System.Windows.Forms;

namespace Asocijacije {
    partial class MyForm {

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
            this.closeBtn = new System.Windows.Forms.PictureBox();
            this.fullscreenBtn = new System.Windows.Forms.PictureBox();
            this.minimizeBtn = new System.Windows.Forms.PictureBox();
            this.backBtn = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.closeBtn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fullscreenBtn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minimizeBtn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.backBtn)).BeginInit();
            this.SuspendLayout();
            // 
            // closeBtn
            // 
            this.closeBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.closeBtn.BackColor = System.Drawing.Color.Transparent;
            this.closeBtn.Image = global::Asocijacije.Properties.Resources.close;
            this.closeBtn.Location = new System.Drawing.Point(1205, 0);
            this.closeBtn.Margin = new System.Windows.Forms.Padding(0);
            this.closeBtn.Name = "closeBtn";
            this.closeBtn.Size = new System.Drawing.Size(75, 50);
            this.closeBtn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.closeBtn.TabIndex = 2;
            this.closeBtn.TabStop = false;
            this.closeBtn.Click += new System.EventHandler(this.Close_Click);
            this.closeBtn.MouseEnter += new System.EventHandler(this.Close_MouseEnter);
            this.closeBtn.MouseLeave += new System.EventHandler(this.Close_MouseLeave);
            // 
            // fullscreenBtn
            // 
            this.fullscreenBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.fullscreenBtn.BackColor = System.Drawing.Color.Transparent;
            this.fullscreenBtn.Image = global::Asocijacije.Properties.Resources.fullscreen1;
            this.fullscreenBtn.Location = new System.Drawing.Point(1130, 0);
            this.fullscreenBtn.Margin = new System.Windows.Forms.Padding(0);
            this.fullscreenBtn.Name = "fullscreenBtn";
            this.fullscreenBtn.Size = new System.Drawing.Size(75, 50);
            this.fullscreenBtn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.fullscreenBtn.TabIndex = 2;
            this.fullscreenBtn.TabStop = false;
            this.fullscreenBtn.Click += new System.EventHandler(this.Fullscreen_Click);
            this.fullscreenBtn.MouseEnter += new System.EventHandler(this.Fullscreen_MouseEnter);
            this.fullscreenBtn.MouseLeave += new System.EventHandler(this.Fullscreen_MouseLeave);
            // 
            // minimizeBtn
            // 
            this.minimizeBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.minimizeBtn.BackColor = System.Drawing.Color.Transparent;
            this.minimizeBtn.Image = global::Asocijacije.Properties.Resources.minimize;
            this.minimizeBtn.Location = new System.Drawing.Point(1055, 0);
            this.minimizeBtn.Margin = new System.Windows.Forms.Padding(0);
            this.minimizeBtn.Name = "minimizeBtn";
            this.minimizeBtn.Size = new System.Drawing.Size(75, 50);
            this.minimizeBtn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.minimizeBtn.TabIndex = 2;
            this.minimizeBtn.TabStop = false;
            this.minimizeBtn.Click += new System.EventHandler(this.Minimize_Click);
            this.minimizeBtn.MouseEnter += new System.EventHandler(this.Minimize_MouseEnter);
            this.minimizeBtn.MouseLeave += new System.EventHandler(this.Minimize_MouseLeave);
            // 
            // backBtn
            // 
            this.backBtn.BackColor = System.Drawing.Color.Transparent;
            this.backBtn.Image = global::Asocijacije.Properties.Resources.back;
            this.backBtn.Location = new System.Drawing.Point(0, 0);
            this.backBtn.Margin = new System.Windows.Forms.Padding(0);
            this.backBtn.Name = "backBtn";
            this.backBtn.Size = new System.Drawing.Size(75, 50);
            this.backBtn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.backBtn.TabIndex = 3;
            this.backBtn.TabStop = false;
            this.backBtn.Visible = false;
            this.backBtn.MouseEnter += new System.EventHandler(this.BackBtn_MouseEnter);
            this.backBtn.MouseLeave += new System.EventHandler(this.BackBtn_MouseLeave);
            // 
            // MyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1280, 720);
            this.Controls.Add(this.backBtn);
            this.Controls.Add(this.closeBtn);
            this.Controls.Add(this.fullscreenBtn);
            this.Controls.Add(this.minimizeBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(1280, 720);
            this.Name = "MyForm";
            this.Load += new System.EventHandler(this.MyForm_Load);
            this.Resize += new System.EventHandler(this.MyForm_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.closeBtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fullscreenBtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minimizeBtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.backBtn)).EndInit();
            this.ResumeLayout(false);

        }

        private PictureBox closeBtn;
        private PictureBox fullscreenBtn;
        private PictureBox minimizeBtn;
        private PictureBox backBtn;
    }

    #endregion
}