using ChochoNest.Controller;
using ChochoNest.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;

namespace ChochoNest.View
{
    public partial class Keranjang : Form
    {
        private List<int> _listIdProduk;

        // Controller Produk (untuk ambil data produk)
        private readonly ProdukContext _produkController = new ProdukContext();

        // Controller Transaksi (untuk simpan transaksi & potong stok)
        private readonly TransaksiContext _transaksiController;

        private List<KeranjangItem> _keranjangItems = new List<KeranjangItem>();
        private User _loggedInUser;

        // Komponen UI
        private FlowLayoutPanel panelKeranjang;
        private Label labelTotal;
        private Button btnBayar;

        // Constructor
        public Keranjang(User user, List<int> listIdProduk)
        {
            InitializeComponent();

            _loggedInUser = user;
            _listIdProduk = listIdProduk ?? new List<int>();

            // Setup Controller Transaksi dengan koneksi dari DbContext
            ChochoNest.Database.DbContext db = new ChochoNest.Database.DbContext();
            _transaksiController = new TransaksiContext(db.connStr);

            SetupKeranjangUI();
        }

        private void SetupKeranjangUI()
        {
            // Cari komponen existing (jika ada) atau buat baru
            panelKeranjang = this.Controls.Find("flowLayoutPanel1", true).FirstOrDefault() as FlowLayoutPanel;
            labelTotal = this.Controls.Find("label2", true).FirstOrDefault() as Label;
            btnBayar = this.Controls.Find("button1", true).FirstOrDefault() as Button;

            if (panelKeranjang == null)
            {
                panelKeranjang = new FlowLayoutPanel
                {
                    Name = "flowLayoutPanel1",
                    Location = new Point(20, 20),
                    Size = new Size(760, 400),
                    AutoScroll = true,
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle
                };
                this.Controls.Add(panelKeranjang);
            }

            if (labelTotal == null)
            {
                Label lblTotalText = new Label
                {
                    Text = "Total:",
                    Location = new Point(20, 435),
                    Size = new Size(60, 25),
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    TextAlign = ContentAlignment.MiddleLeft
                };
                this.Controls.Add(lblTotalText);

                labelTotal = new Label
                {
                    Name = "label2",
                    Text = "Rp 0",
                    Location = new Point(85, 435),
                    Size = new Size(200, 25),
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    ForeColor = Color.Green,
                    TextAlign = ContentAlignment.MiddleLeft
                };
                this.Controls.Add(labelTotal);
            }

            if (btnBayar == null)
            {
                btnBayar = new Button
                {
                    Name = "button1",
                    Text = "Bayar",
                    Location = new Point(620, 430),
                    Size = new Size(160, 40),
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    BackColor = Color.Green,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand
                };
                this.Controls.Add(btnBayar);
            }

            // Pastikan event handler tidak double
            btnBayar.Click -= button1_Click;
            btnBayar.Click += button1_Click;

            this.Size = new Size(820, 540);
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void Keranjang_Load(object sender, EventArgs e)
        {
            LoadKeranjang();
        }

        private void LoadKeranjang()
        {
            if (_listIdProduk == null || _listIdProduk.Count == 0)
            {
                MessageBox.Show("Keranjang masih kosong.");
                return;
            }

            // Grouping ID produk biar gak double, hitung quantity-nya
            var items = _listIdProduk
                .GroupBy(x => x)
                .Select(g => new { IdProduk = g.Key, Qty = g.Count() })
                .ToList();

            _keranjangItems.Clear();

            foreach (var item in items)
            {
                Produk? p = _produkController.GetProdukById(item.IdProduk);
                if (p != null)
                {
                    int qtyFinal = item.Qty;
                    // Cek stok apakah cukup
                    if (qtyFinal > p.Stok)
                    {
                        MessageBox.Show($"Stok {p.NamaProduk} hanya sisa {p.Stok}!", "Info Stok");
                        qtyFinal = p.Stok;
                    }

                    if (qtyFinal > 0)
                    {
                        _keranjangItems.Add(new KeranjangItem
                        {
                            ProdukData = p,
                            Qty = qtyFinal
                        });
                    }
                }
            }
            TampilkanKeranjang();
        }

        private void TampilkanKeranjang()
        {
            if (panelKeranjang == null) return;

            panelKeranjang.SuspendLayout();
            panelKeranjang.Controls.Clear();

            int totalSemua = 0;

            foreach (var item in _keranjangItems)
            {
                Panel card = BuatCardItem(item);
                panelKeranjang.Controls.Add(card);
                totalSemua += item.GetSubtotal();
            }

            panelKeranjang.ResumeLayout();

            if (labelTotal != null)
                labelTotal.Text = "Rp " + totalSemua.ToString("N0");
        }

        private Panel BuatCardItem(KeranjangItem item)
        {
            int cardWidth = panelKeranjang != null ? panelKeranjang.Width - 30 : 720;

            Panel card = new Panel
            {
                Size = new Size(cardWidth, 100),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(5),
                Padding = new Padding(10)
            };

            Label lblNama = new Label { Text = item.ProdukData?.NamaProduk ?? "Unknown", Location = new Point(10, 10), Size = new Size(300, 25), Font = new Font("Segoe UI", 11, FontStyle.Bold) };
            Label lblHarga = new Label { Text = "@ Rp " + (item.ProdukData?.Harga ?? 0).ToString("N0"), Location = new Point(10, 40), ForeColor = Color.Gray, Font = new Font("Segoe UI", 9), AutoSize = true };
            Label lblSubtotal = new Label { Text = "Subtotal: Rp " + item.GetSubtotal().ToString("N0"), Location = new Point(10, 65), Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.Green, AutoSize = true };

            int btnStartX = card.Width - 180;

            // Tombol -
            Button btnMin = new Button { Text = "-", Size = new Size(40, 40), Location = new Point(btnStartX, 30), Font = new Font("Segoe UI", 16, FontStyle.Bold), BackColor = Color.FromArgb(255, 200, 200), FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };

            // Label Qty
            Label lblQty = new Label { Text = item.Qty.ToString(), Location = new Point(btnStartX + 45, 32), Size = new Size(50, 35), TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 14, FontStyle.Bold), BackColor = Color.WhiteSmoke, BorderStyle = BorderStyle.FixedSingle };

            // Tombol +
            Button btnPlus = new Button { Text = "+", Size = new Size(40, 40), Location = new Point(btnStartX + 100, 30), Font = new Font("Segoe UI", 16, FontStyle.Bold), BackColor = Color.FromArgb(200, 255, 200), FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };

            // Tombol Hapus
            Button btnDel = new Button { Text = "✕", Size = new Size(30, 30), Location = new Point(card.Width - 40, 8), ForeColor = Color.Red, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold), Cursor = Cursors.Hand, BackColor = Color.White };
            btnDel.FlatAppearance.BorderSize = 0;

            // Events
            btnPlus.Click += (s, e) =>
            {
                if (item.ProdukData != null && item.Qty < item.ProdukData.Stok)
                {
                    item.Qty++;
                    TampilkanKeranjang();
                }
                else
                {
                    MessageBox.Show("Stok mentok bang!", "Info");
                }
            };

            btnMin.Click += (s, e) =>
            {
                if (item.Qty > 1)
                {
                    item.Qty--;
                    TampilkanKeranjang();
                }
            };

            btnDel.Click += (s, e) =>
            {
                _keranjangItems.Remove(item);

                // Hapus juga dari list ID utama biar sinkron
                if (item.ProdukData != null)
                {
                    _listIdProduk.RemoveAll(id => id == item.ProdukData.IdProduk);
                }

                TampilkanKeranjang();
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

        private void button1_Click(object sender, EventArgs e)
        {
            if (_keranjangItems.Count == 0)
            {
                MessageBox.Show("Keranjang kosong!", "Info");
                return;
            }

            int total = 0;
            foreach (var item in _keranjangItems) total += item.GetSubtotal();

            // Form Popup Pembayaran
            using (Form formBayar = new Form())
            {
                formBayar.Text = "Pembayaran";
                formBayar.Size = new Size(400, 300);
                formBayar.StartPosition = FormStartPosition.CenterParent;
                formBayar.FormBorderStyle = FormBorderStyle.FixedDialog;
                formBayar.MaximizeBox = false;

                Label lblTotal = new Label { Text = $"Total Belanja: Rp {total:N0}", Location = new Point(20, 20), Size = new Size(350, 30), Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = Color.Green };
                Label lblMetode = new Label { Text = "Metode Pembayaran:", Location = new Point(20, 60), Size = new Size(150, 20) };

                ComboBox cmbMetode = new ComboBox { Location = new Point(180, 58), Size = new Size(180, 25), DropDownStyle = ComboBoxStyle.DropDownList };
                cmbMetode.Items.AddRange(new object[] { "Tunai", "QRIS" });
                cmbMetode.SelectedIndex = 0;

                Label lblBayar = new Label { Text = "Uang Bayar:", Location = new Point(20, 100), Size = new Size(150, 20) };
                TextBox txtBayar = new TextBox { Location = new Point(180, 98), Size = new Size(180, 25), Text = total.ToString() };
                Label lblKembali = new Label { Text = "Kembalian: Rp 0", Location = new Point(20, 140), Size = new Size(350, 25), Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.Blue };

                Button btnProses = new Button { Text = "Proses Pembayaran", Location = new Point(80, 190), Size = new Size(220, 40), BackColor = Color.Green, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11, FontStyle.Bold) };

                // Hitung kembalian real-time
                txtBayar.TextChanged += (s, ev) =>
                {
                    if (int.TryParse(txtBayar.Text, out int bayar))
                    {
                        int kembali = bayar - total;
                        lblKembali.Text = $"Kembalian: Rp {(kembali >= 0 ? kembali : 0):N0}";
                    }
                };

                // Logic Tombol Proses Bayar
                btnProses.Click += (s, ev) =>
                {
                    if (!int.TryParse(txtBayar.Text, out int bayar) || bayar < total)
                    {
                        MessageBox.Show("Uang bayar kurang, Bos!", "Waduh", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    int kembali = bayar - total;
                    string metode = cmbMetode.SelectedItem?.ToString() ?? "Tunai";

                    try
                    {
                        // 1. Simpan Transaksi & Update Stok
                        SimpanTransaksi(total, metode);

                        // 2. Cetak Struk (Ini yang tadi hilang)
                        CetakStruk(bayar, kembali, metode);

                        // 3. Bersihkan keranjang
                        _keranjangItems.Clear();
                        _listIdProduk.Clear();
                        TampilkanKeranjang();

                        MessageBox.Show("Transaksi Berhasil! Stok sudah berkurang.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        formBayar.Close();
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("GAGAL: " + ex.Message, "Error Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };

                formBayar.Controls.AddRange(new Control[] { lblTotal, lblMetode, cmbMetode, lblBayar, txtBayar, lblKembali, btnProses });
                formBayar.ShowDialog();
            }
        }

        private void SimpanTransaksi(int total, string metode)
        {
            TransaksiModel trx = new TransaksiModel();
            trx.TotalBayar = total;
            trx.IdUser = _loggedInUser.Id; // Pakai ID User yang login
            trx.IdMetodePembayaran = (metode == "QRIS") ? 2 : 1;

            foreach (var item in _keranjangItems)
            {
                if (item.ProdukData != null)
                {
                    trx.ListDetail.Add(new DetailTransaksi
                    {
                        IdProduk = item.ProdukData.IdProduk,
                        JumlahBeli = item.Qty, // Jumlah beli dikirim ke sini
                        Subtotal = item.GetSubtotal()
                    });
                }
            }
            // Kirim ke Controller
            _transaksiController.SimpanTransaksi(trx);
        }

        // --- METHOD CETAK STRUK (SUDAH DIKEMBALIKAN) ---
        private void CetakStruk(int bayar, int kembali, string metode)
        {
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += (s, e) =>
            {
                if (e.Graphics == null) return;

                Graphics g = e.Graphics;
                Font fontJudul = new Font("Courier New", 14, FontStyle.Bold);
                Font fontRegular = new Font("Courier New", 10, FontStyle.Regular);
                Font fontBold = new Font("Courier New", 10, FontStyle.Bold);

                float y = 20;
                int margin = 20;
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
                foreach (var item in _keranjangItems)
                {
                    if (item.ProdukData == null) continue;

                    // Nama Barang
                    g.DrawString(item.ProdukData.NamaProduk, fontRegular, Brushes.Black, margin, y);
                    y += 15;

                    // Qty x Harga ...... Subtotal
                    string detail = $"{item.Qty} x {item.ProdukData.Harga:N0}";
                    g.DrawString(detail, fontRegular, Brushes.Black, margin, y);

                    // Subtotal di kanan
                    g.DrawString(item.GetSubtotal().ToString("N0"), fontRegular, Brushes.Black, 280, y, right);
                    y += 20;
                }

                g.DrawString("-----------------------------------------", fontRegular, Brushes.Black, margin, y);
                y += 15;

                // 3. TOTAL & PEMBAYARAN
                int total = 0;
                foreach (var x in _keranjangItems)
                    total += x.GetSubtotal();

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
            try
            {
                PrintPreviewDialog preview = new PrintPreviewDialog();
                preview.Document = pd;
                preview.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal print: " + ex.Message);
            }
        }
    }

    public class KeranjangItem
    {
        public Produk? ProdukData { get; set; }
        public int Qty { get; set; }
        public int GetSubtotal()
        {
            return (ProdukData?.Harga ?? 0) * Qty;
        }
    }
}