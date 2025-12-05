using System;
using System.Drawing;
using System.Windows.Forms;

namespace ChochoNest.View
{
    partial class Keranjang
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Initialize minimal designer surface so InitializeComponent() exists.
        /// This keeps runtime/designer happy; the runtime UI is created in code (SetupKeranjangUI).
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.SuspendLayout();
            // 
            // Keranjang
            // 
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(820, 540);
            this.Name = "Keranjang";
            this.Text = "Keranjang";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Load += new EventHandler(this.Keranjang_Load);
            this.ResumeLayout(false);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}