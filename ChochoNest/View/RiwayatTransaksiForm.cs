using ChochoNest.Controller;
using ChochoNest.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace ChochoNest.View
{
    public partial class RiwayatTransaksiForm : Form
    {
        public User LoggedInUser { get; private set; }
        private TransaksiContext _transaksiContext;

        private DataGridView dgvTransactions;
        public RiwayatTransaksiForm(User user)
        {
            InitializeComponent();
            LoggedInUser = user;
            // 1. Buat Objek DbContext (untuk memuat koneksi dari .env)
            ChochoNest.Database.DbContext db = new ChochoNest.Database.DbContext();

            // 2. Ambil connection string dari DbContext (connStr)
            string connectionString = db.connStr;

            // 3. Masukkan connection string ke Controller Transaksi
            _transaksiContext = new TransaksiContext(connectionString);

            this.Text = $"Riwayat Transaksi - {user.Username} ({user.Role})";

            InitializeDataGridView();

            this.Load += RiwayatTransaksiForm_Load;
        }

        private void InitializeDataGridView()
        {
            dgvTransactions = new DataGridView();
            dgvTransactions.Name = "dgvTransactions";
            dgvTransactions.Dock = DockStyle.Fill;
            dgvTransactions.ReadOnly = true;
            dgvTransactions.AutoGenerateColumns = false;
            dgvTransactions.AllowUserToAddRows = false;
            dgvTransactions.RowHeadersVisible = false;
            dgvTransactions.BackgroundColor = System.Drawing.Color.White;
            dgvTransactions.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.WhiteSmoke; // Sedikit lebih terang dari LightGray
            dgvTransactions.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // Tambahan biar enak diklik

            // Kolom ID
            dgvTransactions.Columns.Add("IdTransaksi", "ID");
            dgvTransactions.Columns["IdTransaksi"].DataPropertyName = "IdTransaksi";
            dgvTransactions.Columns["IdTransaksi"].Width = 50;

            // Kolom Tanggal
            dgvTransactions.Columns.Add("TanggalTransaksi", "Tanggal");
            dgvTransactions.Columns["TanggalTransaksi"].DataPropertyName = "TanggalTransaksi";
            dgvTransactions.Columns["TanggalTransaksi"].DefaultCellStyle.Format = "dd MMM yyyy HH:mm"; // Format tanggal cantik
            dgvTransactions.Columns["TanggalTransaksi"].Width = 140;

            // Kolom User
            dgvTransactions.Columns.Add("Username", "User");
            dgvTransactions.Columns["Username"].DataPropertyName = "Username";
            dgvTransactions.Columns["Username"].Width = 100;

            // Kolom Payment
            dgvTransactions.Columns.Add("MetodePembayaran", "Pembayaran");
            dgvTransactions.Columns["MetodePembayaran"].DataPropertyName = "MetodePembayaran";
            dgvTransactions.Columns["MetodePembayaran"].Width = 100;

            // Kolom Total
            dgvTransactions.Columns.Add("TotalBayar", "Total");
            dgvTransactions.Columns["TotalBayar"].DataPropertyName = "TotalBayar";
            dgvTransactions.Columns["TotalBayar"].DefaultCellStyle.Format = "C0"; // Format Currency (Rp)
            dgvTransactions.Columns["TotalBayar"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvTransactions.Columns["TotalBayar"].Width = 120;

            // Kolom Detail (Unbound Column)
            DataGridViewTextBoxColumn detailColumn = new DataGridViewTextBoxColumn();
            detailColumn.Name = "DetailTransaksi"; // Penting untuk pemanggilan nanti
            detailColumn.HeaderText = "Detail Barang";
            detailColumn.ReadOnly = true;
            detailColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; // Biar menuhin layar
            dgvTransactions.Columns.Add(detailColumn);

            this.Controls.Add(dgvTransactions);
        }

        private void RiwayatTransaksiForm_Load(object sender, EventArgs e)
        {
            LoadTransactions();
        }

        private void LoadTransactions()
        {
            try
            {
                List<TransaksiModel> transactions;

                // Panggil fungsi dari Controller baru
                if (LoggedInUser.Role == "admin")
                {
                    transactions = _transaksiContext.GetRiwayatTransaksi(); // Ambil semua
                }
                else
                {
                    transactions = _transaksiContext.GetRiwayatTransaksi(LoggedInUser.Id); // Filter user
                }

                if (!transactions.Any())
                {
                    MessageBox.Show("Belum ada riwayat transaksi.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    dgvTransactions.DataSource = null;
                    return;
                }

                // Masukkan data ke tabel
                dgvTransactions.DataSource = transactions;

                // Loop untuk mengisi kolom "Detail Barang" yang manual
                foreach (DataGridViewRow row in dgvTransactions.Rows)
                {
                    var transaction = row.DataBoundItem as TransaksiModel;

                    if (transaction != null && transaction.ListDetail.Any())
                    {
                        // Contoh format: "Kopi (2x), Roti (1x)"
                        string details = string.Join(", ", transaction.ListDetail.Select(d => $"{d.NamaProduk} ({d.JumlahBeli}x)"));
                        row.Cells["DetailTransaksi"].Value = details;
                    }
                    else
                    {
                        row.Cells["DetailTransaksi"].Value = "-";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data: " + ex.Message);
            }
        }
    }
}