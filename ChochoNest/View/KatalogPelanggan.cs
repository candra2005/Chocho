using ChochoNest.Controller;
using ChochoNest.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ChochoNest.View
{
    public partial class KatalogPelanggan : Form
    {
        public static List<int> Keranjang = new List<int>();
        private readonly ProdukContext _controller = new ProdukContext();
        
        private Panel panelSidebar;
        private Panel panelHeader;
        private FlowLayoutPanel panelProduk;
        
        public User LoggedInUser { get; private set; }

        public KatalogPelanggan(User user)
        {
            InitializeComponent();
            LoggedInUser = user;
            
            SetupLayout();
            LoadProdukToPanel();
        }

        // --- INI BAGIAN YANG TADI HILANG ---
        // Fungsi ini wajib ada karena dipanggil oleh Designer
        private void KatalogPelanggan_Load(object sender, EventArgs e)
        {
            // Biarkan kosong, atau bisa dipakai untuk reload data jika perlu
        }
        // -----------------------------------

        private void SetupLayout()
        {
            this.Text = "ChochoNest - Katalog Pelanggan";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.WhiteSmoke;

            // Setup Header
            panelHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.White
            };

            Label lblWelcome = new Label
            {
                Text = $"Halo, {LoggedInUser.Username}!",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.SaddleBrown,
                AutoSize = true,
                Location = new Point(20, 15)
            };
            panelHeader.Controls.Add(lblWelcome);

            // Setup Sidebar
            panelSidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 220,
                BackColor = Color.SaddleBrown,
                Padding = new Padding(10)
            };

            Button btnKatalog = CreateMenuButton("📦 Katalog Produk", 0);
            btnKatalog.Click += (s, e) => LoadProdukToPanel();

            Button btnKeranjang = CreateMenuButton("🛒 Lihat Keranjang", 50);
            btnKeranjang.Click += KeranjangBTN_Click;

            Button btnRiwayat = CreateMenuButton("📜 Riwayat Transaksi", 100);
            btnRiwayat.Click += BtnRiwayat_Click;

            Button btnLogout = CreateMenuButton("🚪 Logout", 300);
            btnLogout.BackColor = Color.DarkRed;
            btnLogout.Click += (s, e) => { this.Close(); };

            panelSidebar.Controls.Add(btnKatalog);
            panelSidebar.Controls.Add(btnKeranjang);
            panelSidebar.Controls.Add(btnRiwayat);
            panelSidebar.Controls.Add(btnLogout);

            // Setup Content
            panelProduk = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(20),
                BackColor = Color.WhiteSmoke
            };

            this.Controls.Add(panelProduk);
            this.Controls.Add(panelHeader);
            this.Controls.Add(panelSidebar);
        }

        private Button CreateMenuButton(string text, int topPosition)
        {
            Button btn = new Button
            {
                Text = text,
                Top = topPosition + 20,
                Left = 10,
                Width = 200,
                Height = 40,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private void BtnRiwayat_Click(object sender, EventArgs e)
        {
            RiwayatTransaksiForm formRiwayat = new RiwayatTransaksiForm(LoggedInUser);
            formRiwayat.ShowDialog();
        }

        private void KeranjangBTN_Click(object sender, EventArgs e)
        {
            Keranjang formKeranjang = new Keranjang(Keranjang);
            formKeranjang.ShowDialog();
        }

        public void LoadProdukToPanel()
        {
            if (panelProduk == null) return;

            panelProduk.SuspendLayout();
            panelProduk.Controls.Clear();

            try
            {
                var listProduk = _controller.GetProdukFromDatabase();

                if (listProduk.Count == 0)
                {
                    Label lblKosong = new Label { Text = "Belum ada produk.", AutoSize = true, Font = new Font("Segoe UI", 14) };
                    panelProduk.Controls.Add(lblKosong);
                }

                foreach (var item in listProduk)
                {
                    if (item.Stok > 0)
                    {
                        Panel kartu = CreateProdukCard(item);
                        panelProduk.Controls.Add(kartu);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }

            panelProduk.ResumeLayout();
        }

        public void TambahKeKeranjang(int idProduk)
        {
            Keranjang.Add(idProduk);
            MessageBox.Show("Berhasil ditambahkan ke keranjang!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private Panel CreateProdukCard(Produk produk)
        {
            // Use panelProduk background as base so cards follow content background
            Color cardBg = panelProduk?.BackColor ?? this.BackColor;
            Panel card = new Panel
            {
                Size = new Size(200, 300),
                BackColor = cardBg, // adapt to container background
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(15),
            };

            PictureBox pb = new PictureBox
            {
                Size = new Size(180, 130),
                Location = new Point(10, 10),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = cardBg // match card background
            };

            if (produk.GambarProduk != null && produk.GambarProduk.Length > 0)
            {
                try
                {
                    using (var ms = new MemoryStream(produk.GambarProduk))
                    {
                        pb.Image = Image.FromStream(ms);
                    }
                }
                catch { pb.BackColor = Color.Gray; }
            }

            Label lblNama = new Label
            {
                Text = produk.NamaProduk,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(10, 150),
                Size = new Size(180, 40),
                TextAlign = ContentAlignment.TopCenter
            };

            Label lblHarga = new Label
            {
                Text = "Rp " + produk.Harga.ToString("N0"),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.DarkOrange,
                Location = new Point(10, 190),
                Size = new Size(180, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblStok = new Label
            {
                Text = $"Stok: {produk.Stok}",
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.Black,
                Location = new Point(10, 210),
                Size = new Size(180, 15),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Button btnBeli = new Button
            {
                Text = "+ Keranjang",
                Size = new Size(180, 35),
                Location = new Point(10, 240),
                BackColor = Color.SaddleBrown,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Tag = produk.IdProduk
            };
            btnBeli.FlatAppearance.BorderSize = 0;
            btnBeli.Click += (s, e) => TambahKeKeranjang((int)((Button)s).Tag);

            card.Controls.Add(pb);
            card.Controls.Add(lblNama);
            card.Controls.Add(lblHarga);
            card.Controls.Add(lblStok);
            card.Controls.Add(btnBeli);

            return card;
        }
    }
}