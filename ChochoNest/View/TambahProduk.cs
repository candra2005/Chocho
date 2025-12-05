using ChochoNest.Controller;
using ChochoNest.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChochoNest.View
{
    public partial class TambahProduk : Form
    {
        private ProdukContext produkContext;
        private OpenFileDialog openFileDialog;
        public Produk Produk;
        private bool isFormatting = false;
        public TambahProduk(Produk produk)
        {
            InitializeComponent();
            produkContext = new ProdukContext();

            openFileDialog = new OpenFileDialog()
            {
                Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png;*.gif|All Files|*.*",
                Title = "Select product image"
            };
            if (produk == null)
            {
                produk = new Produk();
            }
            this.Produk = produk;
            if (produk.GambarProduk != null && produk.GambarProduk.Length > 0)
            {
                try
                {
                    using (var ms = new MemoryStream(produk.GambarProduk))
                    using (var srcImage = Image.FromStream(ms))
                    {
                        gambarProduk.Image = new Bitmap(srcImage);
                    }
                }
                catch (Exception)
                {
                    gambarProduk.Image = null;
                }
            }
            else
            {
                gambarProduk.Image = null;
            }
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string selectedFileName = openFileDialog.FileName;
                    gambarProduk.Image = new Bitmap(selectedFileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Terjadi kesalahan: " + ex.Message);
                }
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tb_NamaProduk_TextChanged(object sender, EventArgs e)
        {

        }

        private void tb_Harga_TextChanged(object sender, EventArgs e)
        {

        }

        private void tb_stok_TextChanged(object sender, EventArgs e)
        {

        }

        private void gambarProduk_Click(object sender, EventArgs e)
        {

        }

        private void TambahProduk_Load(object sender, EventArgs e)
        {

        }

        private void btn_Simpan_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tb_NamaProduk.Text))
            {
                MessageBox.Show("Nama produk tidak boleh kosong!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(tb_Harga.Text, out int harga))
            {
                MessageBox.Show("Harga harus berupa angka bulat!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(tb_stok.Text, out int stok))
            {
                MessageBox.Show("Stok harus berupa angka bulat!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.Produk.NamaProduk = tb_NamaProduk.Text;
            this.Produk.Harga = harga;
            this.Produk.Stok = stok;

            if (gambarProduk.Image != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    gambarProduk.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    this.Produk.GambarProduk = ms.ToArray();
                }
            }
            else
            {
                this.Produk.GambarProduk = null;
            }

            try
            {
                if (this.Produk.IdProduk == 0)
                {
                    produkContext.TambahProduk(this.Produk);
                    MessageBox.Show("Produk berhasil ditambahkan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    produkContext.EditProduk(this.Produk);
                    MessageBox.Show("Produk berhasil diubah!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menyimpan data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void panel1_Paint_1(object sender, PaintEventArgs e)
        {

        }
    }
}