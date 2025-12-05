namespace ChochoNest.View
{
    partial class FormPembayran
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblTotalTagihan = new Label();
            btnModeTunai = new Button();
            btnModeQRIS = new Button();
            pnlTunai = new Panel();
            btnBayarSelesai = new Button();
            lblKembalian = new Label();
            label1 = new Label();
            tbBayar = new TextBox();
            pnlQRIS = new Panel();
            lblStatusQR = new Label();
            pictQRIS = new PictureBox();
            pnlTunai.SuspendLayout();
            pnlQRIS.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictQRIS).BeginInit();
            SuspendLayout();
            // 
            // lblTotalTagihan
            // 
            lblTotalTagihan.AutoSize = true;
            lblTotalTagihan.Location = new Point(514, 59);
            lblTotalTagihan.Name = "lblTotalTagihan";
            lblTotalTagihan.Size = new Size(83, 20);
            lblTotalTagihan.TabIndex = 0;
            lblTotalTagihan.Text = "Total: Rp  0";
            // 
            // btnModeTunai
            // 
            btnModeTunai.Location = new Point(371, 167);
            btnModeTunai.Name = "btnModeTunai";
            btnModeTunai.Size = new Size(94, 29);
            btnModeTunai.TabIndex = 1;
            btnModeTunai.Text = "Tunai";
            btnModeTunai.UseVisualStyleBackColor = true;
            btnModeTunai.Click += btnModeTunai_Click;
            // 
            // btnModeQRIS
            // 
            btnModeQRIS.Location = new Point(634, 167);
            btnModeQRIS.Name = "btnModeQRIS";
            btnModeQRIS.Size = new Size(94, 29);
            btnModeQRIS.TabIndex = 2;
            btnModeQRIS.Text = "Qris";
            btnModeQRIS.UseVisualStyleBackColor = true;
            btnModeQRIS.Click += btnModeQRIS_Click_1;
            // 
            // pnlTunai
            // 
            pnlTunai.Controls.Add(btnBayarSelesai);
            pnlTunai.Controls.Add(lblKembalian);
            pnlTunai.Controls.Add(label1);
            pnlTunai.Controls.Add(tbBayar);
            pnlTunai.Location = new Point(371, 223);
            pnlTunai.Name = "pnlTunai";
            pnlTunai.Size = new Size(357, 188);
            pnlTunai.TabIndex = 3;
            // 
            // btnBayarSelesai
            // 
            btnBayarSelesai.Location = new Point(121, 144);
            btnBayarSelesai.Name = "btnBayarSelesai";
            btnBayarSelesai.Size = new Size(94, 29);
            btnBayarSelesai.TabIndex = 3;
            btnBayarSelesai.Text = "Bayar";
            btnBayarSelesai.UseVisualStyleBackColor = true;
            btnBayarSelesai.Click += btnBayarSelesai_Click;
            // 
            // lblKembalian
            // 
            lblKembalian.AutoSize = true;
            lblKembalian.Location = new Point(26, 79);
            lblKembalian.Name = "lblKembalian";
            lblKembalian.Size = new Size(117, 20);
            lblKembalian.TabIndex = 2;
            lblKembalian.Text = "Kembalian: Rp 0";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(22, 15);
            label1.Name = "label1";
            label1.Size = new Size(169, 20);
            label1.TabIndex = 1;
            label1.Text = "Masukkan nominal uang";
            label1.Click += label1_Click;
            // 
            // tbBayar
            // 
            tbBayar.Location = new Point(22, 38);
            tbBayar.Name = "tbBayar";
            tbBayar.Size = new Size(125, 27);
            tbBayar.TabIndex = 0;
            // 
            // pnlQRIS
            // 
            pnlQRIS.Controls.Add(lblStatusQR);
            pnlQRIS.Controls.Add(pictQRIS);
            pnlQRIS.Location = new Point(371, 223);
            pnlQRIS.Name = "pnlQRIS";
            pnlQRIS.Size = new Size(357, 188);
            pnlQRIS.TabIndex = 4;
            pnlQRIS.Visible = false;
            // 
            // lblStatusQR
            // 
            lblStatusQR.AutoSize = true;
            lblStatusQR.Location = new Point(133, 15);
            lblStatusQR.Name = "lblStatusQR";
            lblStatusQR.Size = new Size(99, 20);
            lblStatusQR.TabIndex = 1;
            lblStatusQR.Text = "Silahkan Scan";
            // 
            // pictQRIS
            // 
            pictQRIS.Location = new Point(118, 64);
            pictQRIS.Name = "pictQRIS";
            pictQRIS.Size = new Size(125, 62);
            pictQRIS.SizeMode = PictureBoxSizeMode.Zoom;
            pictQRIS.TabIndex = 0;
            pictQRIS.TabStop = false;
            // 
            // FormPembayran
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1263, 468);
            Controls.Add(pnlTunai);
            Controls.Add(btnModeQRIS);
            Controls.Add(btnModeTunai);
            Controls.Add(lblTotalTagihan);
            Controls.Add(pnlQRIS);
            Name = "FormPembayran";
            Text = "FormPembayran";
            Load += FormPembayran_Load;
            pnlTunai.ResumeLayout(false);
            pnlTunai.PerformLayout();
            pnlQRIS.ResumeLayout(false);
            pnlQRIS.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictQRIS).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTotalTagihan;
        private Button btnModeTunai;
        private Button btnModeQRIS;
        private Panel pnlTunai;
        private Label label1;
        private TextBox tbBayar;
        private Button btnBayarSelesai;
        private Label lblKembalian;
        private Panel pnlQRIS;
        private PictureBox pictQRIS;
        private Label lblStatusQR;
    }
}