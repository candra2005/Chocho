namespace ChochoNest.View
{
    partial class AdminDashboard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdminDashboard));
            btn_Dashboard = new Button();
            btn_RiwayatTransaksi = new Button();
            btn_KelolaKatalog = new Button();
            btn_Logout = new Button();
            SuspendLayout();
            // 
            // btn_Dashboard
            // 
            btn_Dashboard.BackColor = Color.FromArgb(128, 64, 0);
            btn_Dashboard.ForeColor = SystemColors.Control;
            btn_Dashboard.Location = new Point(63, 172);
            btn_Dashboard.Name = "btn_Dashboard";
            btn_Dashboard.Size = new Size(124, 29);
            btn_Dashboard.TabIndex = 0;
            btn_Dashboard.Text = "Dashboard";
            btn_Dashboard.UseVisualStyleBackColor = false;
            btn_Dashboard.Click += btn_Dashboard_Click;
            // 
            // btn_RiwayatTransaksi
            // 
            btn_RiwayatTransaksi.BackColor = Color.Beige;
            btn_RiwayatTransaksi.Location = new Point(63, 293);
            btn_RiwayatTransaksi.Name = "btn_RiwayatTransaksi";
            btn_RiwayatTransaksi.Size = new Size(160, 29);
            btn_RiwayatTransaksi.TabIndex = 2;
            btn_RiwayatTransaksi.Text = "Riwayat Transaksi";
            btn_RiwayatTransaksi.UseVisualStyleBackColor = false;
            btn_RiwayatTransaksi.Click += btn_RiwayatTransaksi_Click;
            // 
            // btn_KelolaKatalog
            // 
            btn_KelolaKatalog.BackColor = Color.Cornsilk;
            btn_KelolaKatalog.Location = new Point(63, 233);
            btn_KelolaKatalog.Name = "btn_KelolaKatalog";
            btn_KelolaKatalog.Size = new Size(181, 29);
            btn_KelolaKatalog.TabIndex = 3;
            btn_KelolaKatalog.Text = "Pengelolaan Katalog";
            btn_KelolaKatalog.UseVisualStyleBackColor = false;
            btn_KelolaKatalog.Click += btn_KelolaKatalog_Click;
            // 
            // btn_Logout
            // 
            btn_Logout.BackColor = Color.Maroon;
            btn_Logout.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btn_Logout.ForeColor = Color.White;
            btn_Logout.Location = new Point(63, 650);
            btn_Logout.Name = "btn_Logout";
            btn_Logout.Size = new Size(181, 40);
            btn_Logout.TabIndex = 5;
            btn_Logout.Text = "🚪 Logout";
            btn_Logout.UseVisualStyleBackColor = false;
            btn_Logout.Click += btn_Logout_Click;
            // 
            // AdminDashboard
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            ClientSize = new Size(1280, 720);
            Controls.Add(btn_Logout);
            Controls.Add(btn_KelolaKatalog);
            Controls.Add(btn_RiwayatTransaksi);
            Controls.Add(btn_Dashboard);
            FormBorderStyle = FormBorderStyle.None;
            Name = "AdminDashboard";
            Text = "AdminKatalog";
            Load += AdminDashboard_Load;
            ResumeLayout(false);
        }

        #endregion

        private Button btn_Dashboard;
        private Button btn_RiwayatTransaksi;
        private Button btn_KelolaKatalog;
        private Button btn_Logout;
    }
}