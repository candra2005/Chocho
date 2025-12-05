using ChochoNest.Controller;
using ChochoNest.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChochoNest.View
{
    public partial class Keranjang : Form
    {
        private List<int> _listIdProduk;
        private readonly ProdukContext _produkController = new ProdukContext();
        private List<KeranjangItem> _keranjangItems = new List<KeranjangItem>();

        // FlowLayoutPanel yang akan kita buat secara programmatic jika tidak ada di Designer
        private FlowLayoutPanel panelKeranjang;
        private Label labelTotal;
        private Button btnBayar;

        public Keranjang(List<int> listIdProduk)
        {
            InitializeComponent();
            _listIdProduk = listIdProduk ?? new List<int>();

            // Setup UI
            SetupKeranjangUI();
        }

        private void SetupKeranjangUI()
        {
            // Cari komponen yang mungkin sudah ada di Designer
            panelKeranjang = this.Controls.Find("flowLayoutPanel1", true).FirstOrDefault() as FlowLayoutPanel;
            labelTotal = this.Controls.Find("label2", true).FirstOrDefault() as Label;
            btnBayar = this.Controls.Find("button1", true).FirstOrDefault() as Button;

            // Jika flowLayoutPanel tidak ada, buat baru di area yang tepat
            if (panelKeranjang == null)
            {
                // Buat FlowLayoutPanel untuk menampilkan item keranjang
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

            // Jika label total tidak ada, buat baru
            if (labelTotal == null)
            {
                // Label untuk text "Total:"
                Label lblTotalText = new Label
                {
                    Text = "Total:",
                    Location = new Point(20, 435),
                    Size = new Size(60, 25),
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleLeft
                };
                this.Controls.Add(lblTotalText);

                // Label untuk nilai total
                labelTotal = new Label
                {
                    Name = "label2",
                    Text = "Rp 0",
                    Location = new Point(85, 435),
                    Size = new Size(200, 25),
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    ForeColor = Color.Green,
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleLeft
                };
                this.Controls.Add(labelTotal);
            }

            // Jika button bayar tidak ada, buat baru
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

            // Pastikan button bayar ada event handler
            if (btnBayar != null)
            {
                btnBayar.Click -= button1_Click;
                btnBayar.Click += button1_Click;
            }

            // Set ukuran form yang sesuai
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

            // Group produk berdasarkan ID dan hitung quantity
            var items = _listIdProduk
                .GroupBy(x => x)
                .Select(g => new { IdProduk = g.Key, Qty = g.Count() })
                .ToList();

            foreach (var item in items)
            {
                Produk? p = _produkController.GetProdukById(item.IdProduk);
                if (p != null)
                {
                    if (item.Qty <= p.Stok)
                    {
                        _keranjangItems.Add(new KeranjangItem
                        {
                            ProdukData = p,
                            Qty = item.Qty
                        });
                    }
                    else
                    {
                        MessageBox.Show($"Stok {p.NamaProduk} tidak mencukupi! Tersedia: {p.Stok}", "Peringatan");
                        if (p.Stok > 0)
                        {
                            _keranjangItems.Add(new KeranjangItem
                            {
                                ProdukData = p,
                                Qty = p.Stok
                            });
                        }
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

            // Update label total yang sudah ada di Designer
            if (labelTotal != null)
            {
                labelTotal.Text = "Rp " + totalSemua.ToString("N0");
            }
            else
            {
                // Cari label lain jika labelTotal null
                Control? lblTotal = this.Controls.Find("label2", true).FirstOrDefault();
                if (lblTotal is Label lbl)
                {
                    lbl.Text = "Rp " + totalSemua.ToString("N0");
                }
            }
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

            // Nama Produk
            Label lblNama = new Label
            {
                Text = item.ProdukData?.NamaProduk ?? "Unknown",
                Location = new Point(10, 10),
                Size = new Size(300, 25),
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };

            // Harga Satuan
            Label lblHarga = new Label
            {
                Text = "@ Rp " + (item.ProdukData?.Harga ?? 0).ToString("N0"),
                Location = new Point(10, 40),
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 9),
                AutoSize = true
            };

            // Subtotal
            Label lblSubtotal = new Label
            {
                Text = "Subtotal: Rp " + item.GetSubtotal().ToString("N0"),
                Location = new Point(10, 65),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.Green,
                AutoSize = true
            };

            int btnStartX = card.Width - 180;

            // Tombol Minus (-)
            Button btnMin = new Button
            {
                Text = "-",
                Size = new Size(40, 40),
                Location = new Point(btnStartX, 30),
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                BackColor = Color.FromArgb(255, 200, 200),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnMin.FlatAppearance.BorderColor = Color.FromArgb(200, 100, 100);

            // Label Quantity
            Label lblQty = new Label
            {
                Text = item.Qty.ToString(),
                Location = new Point(btnStartX + 45, 32),
                Size = new Size(50, 35),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                BackColor = Color.WhiteSmoke,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Tombol Plus (+)
            Button btnPlus = new Button
            {
                Text = "+",
                Size = new Size(40, 40),
                Location = new Point(btnStartX + 100, 30),
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                BackColor = Color.FromArgb(200, 255, 200),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnPlus.FlatAppearance.BorderColor = Color.FromArgb(100, 200, 100);

            // Tombol Delete (X)
            Button btnDel = new Button
            {
                Text = "✕",
                Size = new Size(30, 30),
                Location = new Point(card.Width - 40, 8),
                ForeColor = Color.Red,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Cursor = Cursors.Hand,
                BackColor = Color.White
            };
            btnDel.FlatAppearance.BorderSize = 0;

            // Event Handler - Tombol Plus
            btnPlus.Click += (s, e) =>
            {
                if (item.ProdukData != null && item.Qty < item.ProdukData.Stok)
                {
                    item.Qty++;
                    TampilkanKeranjang();
                }
                else
                {
                    MessageBox.Show("Stok tidak mencukupi!", "Info");
                }
            };

            // Event Handler - Tombol Minus
            btnMin.Click += (s, e) =>
            {
                if (item.Qty > 1)
                {
                    item.Qty--;
                    TampilkanKeranjang();
                }
            };

            // Event Handler - Tombol Delete
            btnDel.Click += (s, e) =>
            {
                string namaProduk = item.ProdukData?.NamaProduk ?? "produk ini";
                var result = MessageBox.Show(
                    $"Hapus {namaProduk} dari keranjang?",
                    "Konfirmasi",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    _keranjangItems.Remove(item);
                    TampilkanKeranjang();

                    if (_keranjangItems.Count == 0)
                    {
                        MessageBox.Show("Keranjang kosong!", "Info");
                    }
                }
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
            // Tombol Bayar
            if (_keranjangItems.Count == 0)
            {
                MessageBox.Show("Keranjang kosong!", "Info");
                return;
            }

            int total = 0;
            foreach (var item in _keranjangItems)
                total += item.GetSubtotal();

            // Tampilkan dialog pembayaran sederhana
            using (Form formBayar = new Form())
            {
                formBayar.Text = "Pembayaran";
                formBayar.Size = new Size(400, 300);
                formBayar.StartPosition = FormStartPosition.CenterParent;
                formBayar.FormBorderStyle = FormBorderStyle.FixedDialog;
                formBayar.MaximizeBox = false;
                formBayar.MinimizeBox = false;

                // Label Total
                Label lblTotal = new Label
                {
                    Text = $"Total Belanja: Rp {total:N0}",
                    Location = new Point(20, 20),
                    Size = new Size(350, 30),
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    ForeColor = Color.Green
                };

                // Label Metode Pembayaran
                Label lblMetode = new Label
                {
                    Text = "Metode Pembayaran:",
                    Location = new Point(20, 60),
                    Size = new Size(150, 20),
                    Font = new Font("Segoe UI", 10)
                };

                // ComboBox Metode
                ComboBox cmbMetode = new ComboBox
                {
                    Location = new Point(180, 58),
                    Size = new Size(180, 25),
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Font = new Font("Segoe UI", 10)
                };
                cmbMetode.Items.AddRange(new object[] { "Tunai", "QRIS" });
                cmbMetode.SelectedIndex = 0;

                // Label Uang Bayar
                Label lblBayar = new Label
                {
                    Text = "Uang Bayar:",
                    Location = new Point(20, 100),
                    Size = new Size(150, 20),
                    Font = new Font("Segoe UI", 10)
                };

                // TextBox Uang Bayar
                TextBox txtBayar = new TextBox
                {
                    Location = new Point(180, 98),
                    Size = new Size(180, 25),
                    Font = new Font("Segoe UI", 10),
                    Text = total.ToString()
                };

                // Label Kembalian
                Label lblKembali = new Label
                {
                    Text = "Kembalian: Rp 0",
                    Location = new Point(20, 140),
                    Size = new Size(350, 25),
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    ForeColor = Color.Blue
                };

                // Event untuk hitung kembalian otomatis
                txtBayar.TextChanged += (s, ev) =>
                {
                    if (int.TryParse(txtBayar.Text, out int bayar))
                    {
                        int kembali = bayar - total;
                        lblKembali.Text = $"Kembalian: Rp {(kembali >= 0 ? kembali : 0):N0}";
                        lblKembali.ForeColor = kembali >= 0 ? Color.Blue : Color.Red;
                    }
                    else
                    {
                        lblKembali.Text = "Kembalian: Rp 0";
                    }
                };

                // Button Proses Bayar
                Button btnProses = new Button
                {
                    Text = "Proses Pembayaran",
                    Location = new Point(80, 190),
                    Size = new Size(220, 40),
                    BackColor = Color.Green,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand
                };

                btnProses.Click += (s, ev) =>
                {
                    if (!int.TryParse(txtBayar.Text, out int bayar) || bayar < total)
                    {
                        MessageBox.Show("Uang bayar tidak mencukupi!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    int kembali = bayar - total;
                    string metode = cmbMetode.SelectedItem?.ToString() ?? "Tunai";

                    // Simpan transaksi ke database (jika ada)
                    try
                    {
                        SimpanTransaksi(total, metode);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Gagal menyimpan transaksi: " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    // Cetak Struk
                    CetakStruk(bayar, kembali, metode);

                    // Bersihkan keranjang
                    _keranjangItems.Clear();
                    TampilkanKeranjang();

                    MessageBox.Show("Transaksi berhasil!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    formBayar.Close();
                    this.Close();
                };

                // Tambahkan kontrol ke form
                formBayar.Controls.Add(lblTotal);
                formBayar.Controls.Add(lblMetode);
                formBayar.Controls.Add(cmbMetode);
                formBayar.Controls.Add(lblBayar);
                formBayar.Controls.Add(txtBayar);
                formBayar.Controls.Add(lblKembali);
                formBayar.Controls.Add(btnProses);

                formBayar.ShowDialog();
            }
        }

        private void SimpanTransaksi(int total, string metode)
        {
            // Simpan ke database jika ada ProdukContext dengan method SimpanTransaksi
            // Uncomment dan sesuaikan jika ada
            /*
            try
            {
                TransaksiModel trx = new TransaksiModel();
                trx.TotalBayar = total;
                trx.IdUser = LoggedInUser.Id;
                trx.IdMetodePembayaran = (metode == "QRIS") ? 2 : 1;

                foreach (var item in _keranjangItems)
                {
                    if (item.ProdukData != null)
                    {
                        trx.ListDetail.Add(new DetailTransaksi
                        {
                            IdProduk = item.ProdukData.IdProduk,
                            JumlahBeli = item.Qty,
                            Subtotal = item.GetSubtotal()
                        });
                    }
                }

                _produkController.SimpanTransaksi(trx);
            }
            catch (Exception ex)
            {
                throw new Exception("Gagal menyimpan ke database: " + ex.Message);
            }
            */
        }

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
            PrintPreviewDialog preview = new PrintPreviewDialog();
            preview.Document = pd;
            preview.ShowDialog();
        }
    }

    // Helper Class
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