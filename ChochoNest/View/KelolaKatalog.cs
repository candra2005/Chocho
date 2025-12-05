using ChochoNest.Models;
using ChochoNest.Controller;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ChochoNest.View
{
    public partial class KelolaKatalog : Form
    {
        private readonly ProdukContext _controller = new ProdukContext();
        public User LoggedInUser { get; set; }

        public KelolaKatalog(User user = null)
        {
            InitializeComponent();
            LoggedInUser = user;
            LoadProdukToPanel();
            InitializeNavigationButtons();
        }

        private void InitializeNavigationButtons()
        {
            // Create a panel for navigation buttons at the top
            Panel navPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.FromArgb(245, 245, 245),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Dashboard Button
            Button btnDashboard = new Button
            {
                Text = "Dashboard",
                Size = new Size(120, 35),
                Location = new Point(10, 8),
                BackColor = Color.SaddleBrown,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Cursor = Cursors.Hand
            };
            btnDashboard.Click += BtnDashboard_Click;

            // Riwayat Transaksi Button
            Button btnRiwayatTransaksi = new Button
            {
                Text = "Riwayat Transaksi",
                Size = new Size(140, 35),
                Location = new Point(140, 8),
                BackColor = Color.SaddleBrown,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Cursor = Cursors.Hand
            };
            btnRiwayatTransaksi.Click += BtnRiwayatTransaksi_Click;

            // Logout Button
            Button btnLogout = new Button
            {
                Text = "Logout",
                Size = new Size(120, 35),
                Location = new Point(290, 8),
                BackColor = Color.Maroon,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Cursor = Cursors.Hand
            };
            btnLogout.Click += BtnLogout_Click;

            navPanel.Controls.Add(btnDashboard);
            navPanel.Controls.Add(btnRiwayatTransaksi);
            navPanel.Controls.Add(btnLogout);

            this.Controls.Add(navPanel);
            this.Controls.SetChildIndex(navPanel, 0);
        }

        private void BtnDashboard_Click(object sender, EventArgs e)
        {
            if (LoggedInUser != null)
            {
                AdminDashboard dashboard = new AdminDashboard(LoggedInUser);
                dashboard.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("User information not available.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRiwayatTransaksi_Click(object sender, EventArgs e)
        {
            if (LoggedInUser != null)
            {
                RiwayatTransaksiForm riwayatForm = new RiwayatTransaksiForm(LoggedInUser);
                riwayatForm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("User information not available.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Yakin ingin logout?",
                "Konfirmasi Logout",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                LoginForm loginForm = new LoginForm();
                loginForm.Show();
                this.Close();
            }
        }

        private void btn_TambahProduk_Click(object sender, EventArgs e)
        {
            TambahProduk tambahProduk = new TambahProduk(null);

            if (tambahProduk.ShowDialog() == DialogResult.OK)
            {
                LoadProdukToPanel();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadProdukToPanel();
        }

        public void LoadProdukToPanel()
        {
            panelProduk.SuspendLayout();
            panelProduk.Controls.Clear();

            try
            {
                var listProduk = _controller.GetProdukFromDatabase();

                foreach (var item in listProduk)
                {
                    Panel kartu = CreateProdukCard(item);
                    panelProduk.Controls.Add(kartu);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }

            panelProduk.ResumeLayout();
        }

        private Panel CreateProdukCard(Produk produk)
        {
            Panel card = new Panel
            {
                Size = new Size(220, 320),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(10)
            };

            PictureBox pb = new PictureBox
            {
                Size = new Size(180, 140),
                Location = new Point(20, 10),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.LightGray,
                BorderStyle = BorderStyle.FixedSingle
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
                catch { pb.BackColor = Color.Red; }
            }

            Label lblNama = new Label
            {
                Text = produk.NamaProduk,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(10, 160),
                Size = new Size(200, 40),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblHarga = new Label
            {
                Text = "Rp " + produk.Harga.ToString("N0"),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.DarkOrange,
                Location = new Point(10, 200),
                Size = new Size(200, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblStok = new Label
            {
                Text = $"Stok: {produk.Stok}",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                Location = new Point(10, 225),
                Size = new Size(200, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };

            if (produk.Stok <= 0)
            {
                lblStok.ForeColor = Color.Red;
                lblStok.Text = "HABIS";
            }

            Button btnEdit = new Button
            {
                Text = "Edit",
                Size = new Size(85, 30),
                Location = new Point(15, 260),
                BackColor = Color.SaddleBrown,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            Button btnHapus = new Button
            {
                Text = "Hapus",
                Size = new Size(85, 30),
                Location = new Point(110, 260),
                BackColor = Color.Maroon,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btnEdit.Click += (s, e) =>
            {
                TambahProduk form = new TambahProduk(produk);
                if (form.ShowDialog() == DialogResult.OK)
                    LoadProdukToPanel();
            };

            btnHapus.Click += (s, e) =>
            {
                if (MessageBox.Show($"Yakin hapus {produk.NamaProduk}?", "Konfirmasi", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _controller.HapusProduk(produk.IdProduk);
                    LoadProdukToPanel();
                }
            };

            card.Controls.Add(pb);
            card.Controls.Add(lblNama);
            card.Controls.Add(lblHarga);
            card.Controls.Add(lblStok);
            card.Controls.Add(btnEdit);
            card.Controls.Add(btnHapus);

            return card;
        }

        private void panelProduk_Paint(object sender, PaintEventArgs e)
        {
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void KelolaKatalog_Load(object sender, EventArgs e)
        {
        }

        private void btn_Dashboard_Click(object sender, EventArgs e)
        {
        }
    }
}