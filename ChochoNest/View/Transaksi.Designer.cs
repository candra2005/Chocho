namespace ChochoNest.View
{
    partial class Transaksi
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Transaksi));
            btn_Dashboard = new Button();
            button1 = new Button();
            button2 = new Button();
            flpKatalog = new FlowLayoutPanel();
            panel1 = new Panel();
            flpKeranjang = new FlowLayoutPanel();
            panel2 = new Panel();
            lblTotalHarga = new Label();
            btn_Bayar = new Button();
            flpKatalog.SuspendLayout();
            flpKeranjang.SuspendLayout();
            SuspendLayout();
            // 
            // btn_Dashboard
            // 
            btn_Dashboard.Location = new Point(89, 178);
            btn_Dashboard.Name = "btn_Dashboard";
            btn_Dashboard.Size = new Size(94, 29);
            btn_Dashboard.TabIndex = 0;
            btn_Dashboard.Text = "Dashboard";
            btn_Dashboard.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Location = new Point(78, 245);
            button1.Name = "button1";
            button1.Size = new Size(142, 29);
            button1.TabIndex = 1;
            button1.Text = "Kelola Katalog";
            button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.Location = new Point(89, 312);
            button2.Name = "button2";
            button2.Size = new Size(94, 29);
            button2.TabIndex = 2;
            button2.Text = "Transaksi";
            button2.UseVisualStyleBackColor = true;
            // 
            // flpKatalog
            // 
            flpKatalog.AutoScroll = true;
            flpKatalog.BackColor = Color.Transparent;
            flpKatalog.Controls.Add(panel1);
            flpKatalog.Location = new Point(318, 178);
            flpKatalog.Name = "flpKatalog";
            flpKatalog.Size = new Size(697, 383);
            flpKatalog.TabIndex = 3;
            flpKatalog.Paint += flpKatalog_Paint;
            // 
            // panel1
            // 
            panel1.Location = new Point(3, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(250, 125);
            panel1.TabIndex = 0;
            // 
            // flpKeranjang
            // 
            flpKeranjang.AutoScroll = true;
            flpKeranjang.BackColor = Color.Transparent;
            flpKeranjang.Controls.Add(panel2);
            flpKeranjang.FlowDirection = FlowDirection.TopDown;
            flpKeranjang.Location = new Point(1015, 178);
            flpKeranjang.Name = "flpKeranjang";
            flpKeranjang.Size = new Size(250, 383);
            flpKeranjang.TabIndex = 4;
            flpKeranjang.WrapContents = false;
            // 
            // panel2
            // 
            panel2.Location = new Point(3, 3);
            panel2.Name = "panel2";
            panel2.Size = new Size(190, 125);
            panel2.TabIndex = 0;
            // 
            // lblTotalHarga
            // 
            lblTotalHarga.AutoSize = true;
            lblTotalHarga.Location = new Point(1015, 562);
            lblTotalHarga.Name = "lblTotalHarga";
            lblTotalHarga.Size = new Size(87, 20);
            lblTotalHarga.TabIndex = 5;
            lblTotalHarga.Text = "Total Harga";
            // 
            // btn_Bayar
            // 
            btn_Bayar.Location = new Point(1098, 607);
            btn_Bayar.Name = "btn_Bayar";
            btn_Bayar.Size = new Size(94, 29);
            btn_Bayar.TabIndex = 6;
            btn_Bayar.Text = "Bayar";
            btn_Bayar.UseVisualStyleBackColor = true;
            btn_Bayar.Click += btn_Bayar_Click;
            // 
            // Transaksi
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            ClientSize = new Size(1262, 673);
            Controls.Add(btn_Bayar);
            Controls.Add(lblTotalHarga);
            Controls.Add(flpKeranjang);
            Controls.Add(flpKatalog);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(btn_Dashboard);
            Name = "Transaksi";
            Text = "Transaksi";
            Load += Transaksi_Load_1;
            flpKatalog.ResumeLayout(false);
            flpKeranjang.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btn_Dashboard;
        private Button button1;
        private Button button2;
        private FlowLayoutPanel flpKatalog;
        private Panel panel1;
        private FlowLayoutPanel flpKeranjang;
        private Panel panel2;
        private Label lblTotalHarga;
        private Button btn_Bayar;
    }
}