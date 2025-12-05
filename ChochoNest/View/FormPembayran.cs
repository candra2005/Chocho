using System;
using System.Drawing;
using System.Windows.Forms;
using QRCoder; 

namespace ChochoNest.View
{
    public partial class FormPembayran : Form
    {
        private int _totalBelanja;
        private System.Windows.Forms.Timer? _timerSimulasi;
        private int _detikTunggu = 5;
        public int NominalBayar { get; private set; }
        public int NominalKembali { get; private set; }
        public string? MetodeDipilih { get; private set; }

        public FormPembayran(int total)
        {
            InitializeComponent();
            _totalBelanja = total;
            if (lblTotalTagihan != null)
                lblTotalTagihan.Text = "Total Tagihan: Rp " + total.ToString("N0");

            TampilModeTunai();
        }
        private void btnModeTunai_Click(object sender, EventArgs e)
        {
            TampilModeTunai();
        }

        private void TampilModeTunai()
        {
            pnlTunai.Visible = true;
            pnlQRIS.Visible = false;

            btnModeTunai.BackColor = Color.SaddleBrown;
            btnModeTunai.ForeColor = Color.White;

            btnModeQRIS.BackColor = Color.White;
            btnModeQRIS.ForeColor = Color.Black;

            if (_timerSimulasi != null) _timerSimulasi.Stop();
        }

        private void TampilModeQRIS()
        {
            pnlTunai.Visible = false;
            pnlQRIS.Visible = true;

            btnModeQRIS.BackColor = Color.SaddleBrown;
            btnModeQRIS.ForeColor = Color.White;

            btnModeTunai.BackColor = Color.White;
            btnModeTunai.ForeColor = Color.Black;

            GenerateGambarQR();
            MulaiSimulasiScan();
        }

        private void tbBayar_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbBayar.Text)) return;

            if (int.TryParse(tbBayar.Text, out int uangMasuk))
            {
                int kembalian = uangMasuk - _totalBelanja;
                lblKembalian.Text = "Kembalian: Rp " + kembalian.ToString("N0");

                if (kembalian < 0)
                {
                    lblKembalian.ForeColor = Color.Red;
                    btnBayarSelesai.Enabled = false;
                }
                else
                {
                    lblKembalian.ForeColor = Color.Green;
                    btnBayarSelesai.Enabled = true;
                }
            }
        }

        private void btnBayarSelesai_Click(object sender, EventArgs e)
        {
            if (int.TryParse(tbBayar.Text, out int uangMasuk) && uangMasuk >= _totalBelanja)
            {
                NominalBayar = uangMasuk;
                NominalKembali = uangMasuk - _totalBelanja;
                MetodeDipilih = "TUNAI";

                TransaksiBerhasil();
            }
            else
            {
                MessageBox.Show("Uang kurang!");
            }
        }
        private void GenerateGambarQR()
        {
            string dataQR = $"CHOCHO_{DateTime.Now.Ticks}_RP{_totalBelanja}";

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(dataQR, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);

            pictQRIS.Image = qrCode.GetGraphic(5);
        }

        private void MulaiSimulasiScan()
        {
            _detikTunggu = 5;
            lblStatusQR.Text = "Silakan Scan QR di atas... (5)";
            lblStatusQR.ForeColor = Color.Black;

            _timerSimulasi = new System.Windows.Forms.Timer();
            _timerSimulasi.Interval = 1000;
            _timerSimulasi.Tick += (s, e) =>
            {
                _detikTunggu--;
                lblStatusQR.Text = $"Mengecek pembayaran... ({_detikTunggu})";

                if (_detikTunggu <= 0)
                {
                    _timerSimulasi.Stop();
                    lblStatusQR.Text = "PEMBAYARAN SUKSES!";
                    lblStatusQR.ForeColor = Color.Green;

                    NominalBayar = _totalBelanja;
                    NominalKembali = 0;
                    MetodeDipilih = "QRIS";


                    MessageBox.Show("Pembayaran QRIS Diterima!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    TransaksiBerhasil();
                }
            };
            _timerSimulasi.Start();
        }
        private void TransaksiBerhasil()
        {
            if (_timerSimulasi != null) _timerSimulasi.Stop();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void FormPembayran_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_timerSimulasi != null) _timerSimulasi.Stop();
        }

        private void FormPembayran_Load(object sender, EventArgs e)
        {

        }

        private void btnModeQRIS_Click_1(object sender, EventArgs e)
        {
            TampilModeQRIS();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}