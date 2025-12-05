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
        private ProdukContextc _produkContext;
        private DataGridView? dgvTransactions;

        public RiwayatTransaksiForm(User user)
        {
            InitializeComponent();
            LoggedInUser = user;
            _produkContext = new ProdukContext();
            this.Text = $"Riwayat Transaksi - {user.Username} ({user.Role})";

            InitializeDataGridView();
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
            dgvTransactions.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.LightGray;

            dgvTransactions.Columns.Add("IdTransaksi", "ID Transaksi");
            dgvTransactions.Columns["IdTransaksi"].DataPropertyName = "IdTransaksi";
            dgvTransactions.Columns["IdTransaksi"].Width = 80;

            dgvTransactions.Columns.Add("TanggalTransaksi", "Tanggal");
            dgvTransactions.Columns["TanggalTransaksi"].DataPropertyName = "TanggalTransaksi";
            dgvTransactions.Columns["TanggalTransaksi"].Width = 150;

            dgvTransactions.Columns.Add("Username", "User");
            dgvTransactions.Columns["Username"].DataPropertyName = "Username";
            dgvTransactions.Columns["Username"].Width = 100;

            dgvTransactions.Columns.Add("MetodePembayaran", "Metode Pembayaran");
            dgvTransactions.Columns["MetodePembayaran"].DataPropertyName = "MetodePembayaran";
            dgvTransactions.Columns["MetodePembayaran"].Width = 120;

            dgvTransactions.Columns.Add("TotalBayar", "Total Bayar");
            dgvTransactions.Columns["TotalBayar"].DataPropertyName = "TotalBayar";
            dgvTransactions.Columns["TotalBayar"].DefaultCellStyle.Format = "N0";
            dgvTransactions.Columns["TotalBayar"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvTransactions.Columns["TotalBayar"].Width = 100;

            DataGridViewTextBoxColumn detailColumn = new DataGridViewTextBoxColumn();
            detailColumn.Name = "DetailTransaksi";
            detailColumn.HeaderText = "Detail Barang";
            detailColumn.ReadOnly = true;
            detailColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvTransactions.Columns.Add(detailColumn);


            this.Controls.Add(dgvTransactions);
        }

        private void RiwayatTransaksiForm_Load(object sender, EventArgs e)
        {
            
            LoadTransactions();
        }

        private void LoadTransactions()
        {
            if (dgvTransactions == null)
            {
                MessageBox.Show("DataGridView not initialized.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<TransaksiModel> transactions;

            if (LoggedInUser.Role == "admin")
            {
                transactions = _produkContext.GetRiwayatTransaksi();
            }
            else // Regular user
            {
                transactions = _produkContext.GetRiwayatTransaksi(LoggedInUser.Id);
            }

            if (!transactions.Any())
            {
                MessageBox.Show("No transactions found for the current user/admin.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                dgvTransactions.DataSource = null;
                return;
            }

            dgvTransactions.DataSource = transactions;

            foreach (DataGridViewRow row in dgvTransactions.Rows)
            {
                TransaksiModel? transaction = row.DataBoundItem as TransaksiModel;
                if (transaction != null && transaction.ListDetail.Any())
                {
                    string details = string.Join(", ", transaction.ListDetail.Select(d => $"{d.NamaProduk} ({d.JumlahBeli}x)"));
                    row.Cells["DetailTransaksi"].Value = details;
                }
                else
                {
                    row.Cells["DetailTransaksi"].Value = "No details available";
                }
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}