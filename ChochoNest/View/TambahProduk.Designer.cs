namespace ChochoNest.View
{
    partial class TambahProduk
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
            tb_NamaProduk = new TextBox();
            tb_Harga = new TextBox();
            tb_stok = new TextBox();
            btnUpload = new Button();
            gambarProduk = new PictureBox();
            btn_Simpan = new Button();
            panel1 = new Panel();
            ((System.ComponentModel.ISupportInitialize)gambarProduk).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // tb_NamaProduk
            // 
            tb_NamaProduk.Location = new Point(55, 161);
            tb_NamaProduk.Name = "tb_NamaProduk";
            tb_NamaProduk.Size = new Size(199, 27);
            tb_NamaProduk.TabIndex = 0;
            tb_NamaProduk.TextChanged += tb_NamaProduk_TextChanged;
            // 
            // tb_Harga
            // 
            tb_Harga.Location = new Point(55, 208);
            tb_Harga.Name = "tb_Harga";
            tb_Harga.Size = new Size(199, 27);
            tb_Harga.TabIndex = 1;
            tb_Harga.TextChanged += tb_Harga_TextChanged;
            // 
            // tb_stok
            // 
            tb_stok.Location = new Point(55, 241);
            tb_stok.Name = "tb_stok";
            tb_stok.Size = new Size(199, 27);
            tb_stok.TabIndex = 2;
            tb_stok.TextChanged += tb_stok_TextChanged;
            // 
            // btnUpload
            // 
            btnUpload.BackColor = Color.Transparent;
            btnUpload.Location = new Point(69, 107);
            btnUpload.Name = "btnUpload";
            btnUpload.Size = new Size(162, 29);
            btnUpload.TabIndex = 3;
            btnUpload.Text = "Upload Gambar";
            btnUpload.UseVisualStyleBackColor = false;
            btnUpload.Click += btnUpload_Click;
            // 
            // gambarProduk
            // 
            gambarProduk.Location = new Point(110, 17);
            gambarProduk.Name = "gambarProduk";
            gambarProduk.Size = new Size(84, 84);
            gambarProduk.SizeMode = PictureBoxSizeMode.Zoom;
            gambarProduk.TabIndex = 4;
            gambarProduk.TabStop = false;
            gambarProduk.Click += gambarProduk_Click;
            // 
            // btn_Simpan
            // 
            btn_Simpan.Location = new Point(55, 302);
            btn_Simpan.Name = "btn_Simpan";
            btn_Simpan.Size = new Size(199, 29);
            btn_Simpan.TabIndex = 5;
            btn_Simpan.Text = "Simpan";
            btn_Simpan.UseVisualStyleBackColor = true;
            btn_Simpan.Click += btn_Simpan_Click;
            // 
            // panel1
            // 
            panel1.BackgroundImage = Properties.Resources.form_tambah_produk;
            panel1.BackgroundImageLayout = ImageLayout.Zoom;
            panel1.Controls.Add(gambarProduk);
            panel1.Controls.Add(btn_Simpan);
            panel1.Controls.Add(btnUpload);
            panel1.Controls.Add(tb_stok);
            panel1.Controls.Add(tb_NamaProduk);
            panel1.Controls.Add(tb_Harga);
            panel1.Location = new Point(451, 46);
            panel1.Name = "panel1";
            panel1.Size = new Size(303, 345);
            panel1.TabIndex = 6;
            panel1.Paint += panel1_Paint_1;
            // 
            // TambahProduk
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1203, 463);
            Controls.Add(panel1);
            Name = "TambahProduk";
            Text = "TambahProduk";
            Load += TambahProduk_Load;
            ((System.ComponentModel.ISupportInitialize)gambarProduk).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TextBox tb_NamaProduk;
        private TextBox tb_Harga;
        private TextBox tb_stok;
        private Button btnUpload;
        private PictureBox gambarProduk;
        private Button btn_Simpan;
        private Panel panel1;
    }
}