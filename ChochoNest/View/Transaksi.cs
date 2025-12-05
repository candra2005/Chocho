using ChochoNest.Models;
﻿using System.Drawing.Printing;
﻿using ChochoNest.Controller;
﻿using System;
﻿using System.Collections.Generic;
﻿using System.Drawing;
﻿using System.IO;
﻿using System.Linq;
﻿using System.Windows.Forms;
﻿
﻿namespace ChochoNest.View
﻿{
    public partial class Transaksi : Form
    {
        private readonly ProdukContext _controller = new ProdukContext();
        private List<ItemKeranjang> _keranjang = new List<ItemKeranjang>();
        public int NominalBayar { get; private set; }
        public int NominalKembali { get; private set; }
        public string? MetodeDipilih { get; private set; }
        public User LoggedInUser { get; private set; }

        public Transaksi(User user)
        {
            InitializeComponent();
            LoggedInUser = user;
            LoadKatalogProduk();
        }

        private void LoadKatalogProduk()
        {
            flpKatalog.Controls.Clear();

            try
            {
                var listProduk = _controller.GetProdukFromDatabase();

                foreach (var item in listProduk)
                {
                    Panel kartu = CreateKatalogCard(item);
                    flpKatalog.Controls.Add(kartu);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat katalog: " + ex.Message);
            }
        }

        private Panel CreateKatalogCard(Produk produk)
        {
            Panel card = new Panel
            {
                Size = new Size(150, 240),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(10)
            };

            PictureBox pb = new PictureBox
            {
                Size = new Size(130, 100),
                Location = new Point(10, 10),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.LightGray
            };
            if (produk.GambarProduk != null && produk.GambarProduk.Length > 0)
            {
                try { using (var ms = new MemoryStream(produk.GambarProduk)) pb.Image = Image.FromStream(ms); } catch { }
            }

            Label lblNama = new Label
            {
                Text = produk.NamaProduk,
                Location = new Point(10, 115),
                Size = new Size(130, 40),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            Label lblHarga = new Label
            {
                Text = "Rp " + produk.Harga.ToString("N0"),
                Location = new Point(10, 155),
                Size = new Size(130, 20),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.Orange,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            Label lblStok = new Label
            {
                Text = "Stok: " + produk.Stok.ToString(),
                Location = new Point(10, 175),
                Size = new Size(130, 20),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 8, FontStyle.Regular)
            };

            Button btnTambah = new Button﻿
            {
                Text = "Tambah (+)",
                Size = new Size(130, 30),
                Location = new Point(10, 200),
                BackColor = Color.SaddleBrown,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            if (produk.Stok <= 0)
            {
                lblStok.Text = "STOK HABIS";
                lblStok.ForeColor = Color.Red;

                btnTambah.Enabled = false;
                btnTambah.BackColor = Color.Gray;
                btnTambah.Text = "Habis";
            }
            else
            {
                btnTambah.Click += (s, e) => TambahKeKeranjang(produk);
            }

            card.Controls.Add(pb);
            card.Controls.Add(lblNama);
            card.Controls.Add(lblHarga);
            card.Controls.Add(lblStok);
            card.Controls.Add(btnTambah);

            return card;
        }
        private void TambahKeKeranjang(Produk produkDipilih)
        {
            if (produkDipilih.Stok <= 0)
            {
                MessageBox.Show("Stok Habis!", "Maaf");
                return;
            }

            var itemAda = _keranjang.FirstOrDefault(x => x.Produk.IdProduk == produkDipilih.IdProduk);
            if (itemAda != null)
            {
                if (itemAda.Jumlah < produkDipilih.Stok)
                {
                    itemAda.Jumlah++;
                }
                else
                {
                    MessageBox.Show("Stok tidak mencukupi untuk menambah lagi.", "Info");
                    return;
                }
            }
            else
            {
                _keranjang.Add(new ItemKeranjang { Produk = produkDipilih, Jumlah = 1 });
            }
            UpdateTampilanKeranjang();
        }
        private void UpdateTampilanKeranjang()
        {
            flpKeranjang.SuspendLayout();
            flpKeranjang.Controls.Clear();

            decimal totalSemua = 0;

            foreach (var item in _keranjang)
            {
                Panel kartuKecil = CreateKeranjangCard(item);
                flpKeranjang.Controls.Add(kartuKecil);

                totalSemua += item.Subtotal;
            }

            flpKeranjang.ResumeLayout();

            lblTotalHarga.Text = "Total: Rp " + totalSemua.ToString("N0");
        }

        private Panel CreateKeranjangCard(ItemKeranjang item)
        {
            Panel card = new Panel
            {
                Size = new Size(260, 80),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(5)
            };

            Label lblNama = new Label { Text = item.Produk.NamaProduk, Location = new Point(10, 10), Size = new Size(140, 20), Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            Label lblHarga = new Label { Text = "@ " + item.Produk.Harga.ToString("N0"), Location = new Point(10, 30), ForeColor = Color.Gray };
            Label lblSubtotal = new Label { Text = "Total: " + item.Subtotal.ToString("N0"), Location = new Point(10, 50), Font = new Font("Segoe UI", 8, FontStyle.Italic) };

            Button btnMin = new Button { Text = "-", Size = new Size(25, 25), Location = new Point(160, 30) };             // Label Qty
            Label lblQty = new Label { Text = item.Jumlah.ToString(), Location = new Point(190, 35), Size = new Size(30, 20), TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            Button btnPlus = new Button { Text = "+", Size = new Size(25, 25), Location = new Point(225, 30) };
            Button btnDel = new Button { Text = "x", Size = new Size(20, 20), Location = new Point(235, 2), ForeColor = Color.Red, FlatStyle = FlatStyle.Flat }; btnDel.FlatAppearance.BorderSize = 0;


            btnPlus.Click += (s, e) =>
                  {
                      if (item.Jumlah < item.Produk.Stok) { item.Jumlah++; UpdateTampilanKeranjang(); }
                      else { MessageBox.Show("Mentok Stok!"); }
                  };

            btnMin.Click += (s, e) =>
            {
                if (item.Jumlah > 1) { item.Jumlah--; UpdateTampilanKeranjang(); }
            };

            btnDel.Click += (s, e) =>
            {
                _keranjang.Remove(item);
                UpdateTampilanKeranjang();
            };

            card.Controls.Add(lblNama);
            card.Controls.Add(lblHarga);
            card.Controls.Add(lblSubtotal);
            card.Controls.Add(btnMin);
            card.Controls.Add(lblQty);
            card.Controls.Add(btnPlus);
            card.Controls.Add(btnDel);

            return card;
        }

        // ==========================================
        // BAGIAN 4: TOMBOL BAYAR
        // ==========================================
        // Pastikan event click tombol Bayar nyambung ke sini
        private void btnBayar_Click(object sender, EventArgs e)
        {
            if (_keranjang.Count == 0)
            {
                MessageBox.Show("Keranjang kosong, mau beli apa?");
                return;
            }

            // SIMULASI BAYAR
            string struk = "Konfirmasi Pembelian:\n\n";
            int total = 0;
            foreach (var x in _keranjang)
            {
                struk += $"- {x.Produk.NamaProduk} ({x.Jumlah}x) = {x.Subtotal:N0}\n";
                total += x.Subtotal;

                // DISINI NANTI KITA KURANGI STOK DI DATABASE (TRANSAKSI)
                _controller.TransaksiProduk(x.Produk.IdProduk, x.Jumlah);
            }
            struk += $"\nTOTAL BAYAR: Rp {total:N0}";

            MessageBox.Show("Transaksi Berhasil Disimpan!\n\n" + struk, "Sukses");


            _keranjang.Clear();
            UpdateTampilanKeranjang();
            LoadKatalogProduk(); // Refresh stok di kiri
        }

        private void Transaksi_Load(object sender, EventArgs e)
        {

        }

        private void flpKatalog_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void btn_Bayar_Click(object sender, EventArgs e)
        {
            if (_keranjang.Count == 0)
            {
                MessageBox.Show("Keranjang kosong!");
                return;
            }

            int total = 0;
            foreach (var item in _keranjang) total += item.Subtotal;

            // Panggil Form Pembayaran (Pastikan nama class-nya sesuai typo kamu: FormPembayran)
            FormPembayran formBayar = new FormPembayran(total);

            if (formBayar.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Ambil Data
                    int uangBayar = formBayar.NominalBayar;
                    int uangKembali = formBayar.NominalKembali;
                    string metode = formBayar.MetodeDipilih;

                    // Simpan Database (Pakai TransaksiModel)
                    TransaksiModel trx = new TransaksiModel();
                    trx.TotalBayar = total;
                    trx.IdUser = LoggedInUser.Id;
                    trx.IdMetodePembayaran = (metode == "QRIS") ? 2 : 1;

                    foreach (var item in _keranjang)
                    {
                        trx.ListDetail.Add(new DetailTransaksi
                        {
                            IdProduk = item.Produk.IdProduk,
                            JumlahBeli = item.Jumlah,
                            Subtotal = item.Subtotal
                        });
                    }

                    _controller.SimpanTransaksi(trx);

                    // Cetak Struk
                    CetakStruk(uangBayar, uangKembali, metode);

                    _keranjang.Clear();
                    UpdateTampilanKeranjang();
                    LoadKatalogProduk();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void CetakStruk(int bayar, int kembali, string metode)
        {
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += (s, e) =>
            {
                Graphics g = e.Graphics;
                Font fontJudul = new Font("Courier New", 14, FontStyle.Bold);
                Font fontRegular = new Font("Courier New", 10, FontStyle.Regular);
                Font fontBold = new Font("Courier New", 10, FontStyle.Bold);

                float y = 20;
                int margin = 20;
                int lebarKertas = 300;
                StringFormat center = new StringFormat { Alignment = StringAlignment.Center };
                StringFormat right = new StringFormat { Alignment = StringAlignment.Far };

                // 1. HEADER
                g.DrawString("CHOCHONEST", fontJudul, Brushes.Black, 150, y, center);
                y += 25;
                g.DrawString("Jl. Coklat No. 1, Jember", fontRegular, Brushes.Black, 150, y, center);
                y += 20;
                g.DrawString("-----------------------------------------", fontRegular, Brushes.Black, margin, y);
                y += 15;
                g.DrawString("Tgl: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"), fontRegular, Brushes.Black, margin, y);
                y += 20;
                g.DrawString("-----------------------------------------", fontRegular, Brushes.Black, margin, y);
                y += 15;

                // 2. LIST BARANG
                foreach (var item in _keranjang)
                {
                    // Nama Barang
                    g.DrawString(item.Produk.NamaProduk, fontRegular, Brushes.Black, margin, y);
                    y += 15;

                    // Qty x Harga ...... Subtotal
                    string detail = $"{item.Jumlah} x {item.Produk.Harga:N0}";
                    g.DrawString(detail, fontRegular, Brushes.Black, margin, y);

                    // Subtotal di kanan
                    g.DrawString(item.Subtotal.ToString("N0"), fontRegular, Brushes.Black, 280, y, right);
                    y += 20;
                }

                g.DrawString("-----------------------------------------", fontRegular, Brushes.Black, margin, y);
                y += 15;

                // 3. TOTAL & PEMBAYARAN
                int total = 0;
                foreach (var x in _keranjang) total += x.Subtotal;

                // Total
                g.DrawString("TOTAL :", fontBold, Brushes.Black, 150, y, right);
                g.DrawString(total.ToString("N0"), fontBold, Brushes.Black, 280, y, right);
                y += 20;

                // Bayar
                g.DrawString($"BAYAR ({metode}) :", fontRegular, Brushes.Black, 150, y, right);
                g.DrawString(bayar.ToString("N0"), fontRegular, Brushes.Black, 280, y, right);
                y += 20;

                // Kembali
                g.DrawString("KEMBALI :", fontRegular, Brushes.Black, 150, y, right);
                g.DrawString(kembali.ToString("N0"), fontRegular, Brushes.Black, 280, y, right);
                y += 40;

                // 4. FOOTER
                g.DrawString("Terima Kasih", fontRegular, Brushes.Black, 150, y, center);
                y += 15;
                g.DrawString("Selamat Menikmati", fontRegular, Brushes.Black, 150, y, center);
            };

            // Tampilkan Preview
            PrintPreviewDialog preview = new PrintPreviewDialog();
            preview.Document = pd;
            preview.ShowDialog();
        }

        private void Transaksi_Load_1(object sender, EventArgs e)
        {

        }
    }

    // CLASS PEMBANTU (MODEL KERANJANG)
    public class ItemKeranjang
﻿        {
﻿            public Produk? Produk { get; set; }﻿        public int Jumlah { get; set; }
﻿        public int Subtotal => Produk != null ? Produk.Harga * Jumlah : 0;
﻿
﻿    }
﻿}
﻿