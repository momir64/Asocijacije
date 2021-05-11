using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace Asocijacije {
    partial class TextBoxRounded {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            label.Click -= new EventHandler(OnClick);
            label.MouseDown -= new MouseEventHandler(this.OnMouseDown);
            label.MouseUp -= new MouseEventHandler(this.OnMouseUp);
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.label = new System.Windows.Forms.Label();
            this.textBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label
            // 
            this.label.BackColor = System.Drawing.Color.Transparent;
            this.label.Font = new System.Drawing.Font("Segoe UI", 32F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label.ForeColor = System.Drawing.Color.White;
            this.label.Location = new System.Drawing.Point(89, 27);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(528, 86);
            this.label.TabIndex = 0;
            this.label.Text = "ПРОБА";
            this.label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label.Click += new System.EventHandler(this.OnClick);
            this.label.MouseDown += new MouseEventHandler(this.OnMouseDown);
            this.label.MouseUp += new MouseEventHandler(this.OnMouseUp);
            this.label.TextChanged += new System.EventHandler(this.Label_TextChanged);
            // 
            // textBox
            // 
            this.textBox.Enabled = false;
            this.textBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 0.1F);
            this.textBox.Location = new System.Drawing.Point(73, 52);
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(1, 8);
            this.textBox.TabIndex = 1;
            this.textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox_KeyPress);
            // 
            // TextBoxRounded
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label);
            this.Controls.Add(this.textBox);
            this.Name = "TextBoxRounded";
            this.Size = new System.Drawing.Size(717, 143);
            this.Load += new System.EventHandler(this.TextBoxRounded_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label;
        private TextBox textBox;
    }
}
