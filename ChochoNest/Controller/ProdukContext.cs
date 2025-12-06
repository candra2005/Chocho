
using ChochoNest.Database;
using ChochoNest.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ChochoNest.Controller
{
    public class ProdukContext
    {
        private readonly DbContext _dbContext;
        public event EventHandler? ProdukDiubah;

        public ProdukContext()
        {
            _dbContext = new DbContext();
        }

        protected virtual void OnProdukDiubah(EventArgs e)
        {
            ProdukDiubah?.Invoke(this, e);
        }

        public List<Produk> GetProdukFromDatabase()
        {
            var listProduk = new List<Produk>();
            string query =
                "SELECT id_produk, nama_produk, harga, stok, gambar_produk, is_deleted " +
                "FROM produk ORDER BY id_produk ASC";

            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(_dbContext.connStr))
                {
                    conn.Open();
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Produk p = new Produk();

                            p.IdProduk = Convert.ToInt32(reader["id_produk"]);
                            p.NamaProduk = reader["nama_produk"].ToString()!;
                            p.Harga = Convert.ToInt32(reader["harga"]);
                            p.Stok = Convert.ToInt32(reader["stok"]);
                            p.IsDeleted = Convert.ToBoolean(reader["is_deleted"]);

                            if (reader["gambar_produk"] != DBNull.Value)
                                p.GambarProduk = (byte[])reader["gambar_produk"];

                            listProduk.Add(p);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal ambil data: " + ex.Message);
            }

            return listProduk;
        }

        public void TambahProduk(Produk produk)
        {
            string query =
                "INSERT INTO produk (nama_produk, harga, stok, gambar_produk, is_deleted) " +
                "VALUES (@nama, @harga, @stok, @gambar, FALSE)";

            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(_dbContext.connStr))
                {
                    conn.Open();
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@nama", produk.NamaProduk);
                        cmd.Parameters.AddWithValue("@harga", produk.Harga);
                        cmd.Parameters.AddWithValue("@stok", produk.Stok);

                        if (produk.GambarProduk != null)
                            cmd.Parameters.AddWithValue("@gambar", produk.GambarProduk);
                        else
                            cmd.Parameters.AddWithValue("@gambar", DBNull.Value);

                        cmd.ExecuteNonQuery();
                    }
                }
                OnProdukDiubah(EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal tambah produk: " + ex.Message);
            }
        }

        public void EditProduk(Produk produk)
        {
            string query =
                "UPDATE produk SET nama_produk=@nama, harga=@harga, stok=@stok, gambar_produk=@gambar " +
                "WHERE id_produk=@id";

            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(_dbContext.connStr))
                {
                    conn.Open();
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", produk.IdProduk);
                        cmd.Parameters.AddWithValue("@nama", produk.NamaProduk);
                        cmd.Parameters.AddWithValue("@harga", produk.Harga);
                        cmd.Parameters.AddWithValue("@stok", produk.Stok);

                        if (produk.GambarProduk != null)
                            cmd.Parameters.AddWithValue("@gambar", produk.GambarProduk);
                        else
                            cmd.Parameters.AddWithValue("@gambar", DBNull.Value);

                        cmd.ExecuteNonQuery();
                    }
                }
                OnProdukDiubah(EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal edit produk: " + ex.Message);
            }
        }

        public void HapusProduk(int id)
        {
            string query = "UPDATE produk SET is_deleted = TRUE WHERE id_produk = @id";

            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(_dbContext.connStr))
                {
                    conn.Open();
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
                OnProdukDiubah(EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal hapus produk: " + ex.Message);
            }
        }
        public void UpdateStok(int idProduk, int stok)
        {
            string query = "UPDATE produk SET stok = @stok WHERE id_produk = @id";
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(_dbContext.connStr))
                {
                    conn.Open();
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idProduk);
                        cmd.Parameters.AddWithValue("@stok", stok);
                        cmd.ExecuteNonQuery();
                    }
                }
                OnProdukDiubah(EventArgs.Empty);
            }
            catch (Exception ex) { MessageBox.Show("Gagal update stok: " + ex.Message); }
        }


        public void TransaksiProduk(int idProduk, int jumlahTerjual)
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(_dbContext.connStr))
                {
                    conn.Open();
                    string query = "UPDATE produk SET stok = stok - @jumlah WHERE id_produk = @id";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idProduk);
                        cmd.Parameters.AddWithValue("@jumlah", jumlahTerjual);
                        cmd.ExecuteNonQuery();
                    }
                }
                OnProdukDiubah(EventArgs.Empty);
            }
            catch (Exception ex) { MessageBox.Show("Gagal transaksi stok: " + ex.Message); }
        }


        // --- FITUR SIMPAN TRANSAKSI & UPDATE STOK ---
        public void SimpanTransaksi(TransaksiModel transaksiBaru)
        {
            using (var conn = new NpgsqlConnection(_dbContext.connStr))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        // A. Insert ke Tabel Header 'transaksi'
                        // Kita perlu RETURNING id_transaksi biar dapat ID otomatisnya
                        string queryHeader = @"
                    INSERT INTO transaksi (tanggal_transaksi, total_bayar, id_user, id_metode_pembayaran) 
                    VALUES (@tgl, @total, @idUser, @idMetode) 
                    RETURNING id_transaksi";

                        int idTransaksiBaru;

                        using (var cmd = new NpgsqlCommand(queryHeader, conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@tgl", DateTime.Now);
                            cmd.Parameters.AddWithValue("@total", transaksiBaru.TotalBayar);
                            cmd.Parameters.AddWithValue("@idUser", transaksiBaru.IdUser);

                            // Cek jika metode pembayaran 0 atau null, set default (misal 1: Tunai)
                            int metode = transaksiBaru.IdMetodePembayaran == 0 ? 1 : transaksiBaru.IdMetodePembayaran;
                            cmd.Parameters.AddWithValue("@idMetode", metode);

                            // Eksekusi dan ambil ID baru
                            idTransaksiBaru = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        // B. Loop setiap barang yang dibeli (Detail)
                        foreach (var detail in transaksiBaru.ListDetail)
                        {
                            // 1. Insert ke Tabel 'detail_transaksi'
                            string queryDetail = @"
                        INSERT INTO detail_transaksi (id_transaksi, id_produk, jumlah_beli, jumlah_pembayaran) 
                        VALUES (@idTrans, @idProd, @qty, @subtotal)";

                            using (var cmdDetail = new NpgsqlCommand(queryDetail, conn, tran))
                            {
                                cmdDetail.Parameters.AddWithValue("@idTrans", idTransaksiBaru);
                                cmdDetail.Parameters.AddWithValue("@idProd", detail.IdProduk);
                                cmdDetail.Parameters.AddWithValue("@qty", detail.JumlahBeli);
                                cmdDetail.Parameters.AddWithValue("@subtotal", detail.Subtotal);
                                cmdDetail.ExecuteNonQuery();
                            }

                            // 2. UPDATE STOK PRODUK (INILAH KUNCINYA!) 🔥
                            // Logic: Stok lama dikurangi jumlah beli
                            string queryUpdateStok = @"
                        UPDATE produk 
                        SET stok = stok - @qty 
                        WHERE id_produk = @idProd";

                            using (var cmdStok = new NpgsqlCommand(queryUpdateStok, conn, tran))
                            {
                                cmdStok.Parameters.AddWithValue("@qty", detail.JumlahBeli);
                                cmdStok.Parameters.AddWithValue("@idProd", detail.IdProduk);

                                // Eksekusi update
                                int barisTerubah = cmdStok.ExecuteNonQuery();

                                // Cek error logika (misal ID Produk salah)
                                if (barisTerubah == 0)
                                {
                                    throw new Exception($"Gagal update stok: Produk ID {detail.IdProduk} tidak ditemukan.");
                                }
                            }
                        }

                        // C. Jika semua lancar, COMMIT (Simpan Permanen)
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        // D. Jika ada error sedikitpun, ROLLBACK (Batalkan Semua)
                        tran.Rollback();
                        throw new Exception("Transaksi Gagal Disimpan: " + ex.Message);
                    }
                }
            }
        }


        // --- GANTI FUNGSI GetRiwayatTransaksi DENGAN INI ---
        public List<TransaksiModel> GetRiwayatTransaksi(int? userId = null)
        {
            var transaksiMap = new Dictionary<int, TransaksiModel>();

            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(_dbContext.connStr))
                {
                    conn.Open();

                    // PERBAIKAN: 
                    // 1. Mengubah 'mp.nama' menjadi 'mp.nama_metode_pembayaran'
                    // 2. Menambahkan 't.total_bayar' ke dalam SELECT
                    // 3. Menggunakan LEFT JOIN agar data tetap muncul meski detail/produk terhapus
                    string query = @"
                        SELECT
                            t.id_transaksi,
                            t.tanggal_transaksi,
                            t.id_user,
                            u.username,
                            t.total_bayar, 
                            t.id_metode_pembayaran,
                            mp.nama_metode_pembayaran AS metode_pembayaran_nama,
                            dt.id_detail_transaksi,
                            dt.id_produk,
                            p.nama_produk,
                            p.harga AS harga_satuan,
                            dt.jumlah_beli,
                            dt.jumlah_pembayaran
                        FROM
                            transaksi t
                        JOIN
                            ""user"" u ON t.id_user = u.id_user
                        JOIN
                            metode_pembayaran mp ON t.id_metode_pembayaran = mp.id_metode_pembayaran
                        LEFT JOIN
                            detail_transaksi dt ON t.id_transaksi = dt.id_transaksi
                        LEFT JOIN
                            produk p ON dt.id_produk = p.id_produk";

                    if (userId.HasValue)
                    {
                        query += " WHERE t.id_user = @id_user";
                    }

                    query += " ORDER BY t.tanggal_transaksi DESC, t.id_transaksi DESC;";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        if (userId.HasValue)
                        {
                            cmd.Parameters.AddWithValue("@id_user", userId.Value);
                        }

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int idTransaksi = reader.GetInt32(reader.GetOrdinal("id_transaksi"));

                                // Jika Transaksi belum ada di Dictionary, buat baru (Header)
                                if (!transaksiMap.ContainsKey(idTransaksi))
                                {
                                    TransaksiModel transaksi = new TransaksiModel
                                    {
                                        IdTransaksi = idTransaksi,
                                        TanggalTransaksi = reader.GetDateTime(reader.GetOrdinal("tanggal_transaksi")),
                                        IdUser = reader.GetInt32(reader.GetOrdinal("id_user")),
                                        Username = reader.GetString(reader.GetOrdinal("username")),
                                        IdMetodePembayaran = reader.GetInt32(reader.GetOrdinal("id_metode_pembayaran")),
                                        MetodePembayaran = reader.GetString(reader.GetOrdinal("metode_pembayaran_nama")),

                                        // AMBIL LANGSUNG DARI DB (Lebih Akurat)
                                        TotalBayar = reader.GetInt32(reader.GetOrdinal("total_bayar")),
                                        ListDetail = new List<DetailTransaksi>()
                                    };
                                    transaksiMap.Add(idTransaksi, transaksi);
                                }

                                // Ambil objek transaksi saat ini
                                TransaksiModel currentTransaksi = transaksiMap[idTransaksi];

                                // Cek apakah ada detail barang (antisipasi jika null karena LEFT JOIN)
                                if (!reader.IsDBNull(reader.GetOrdinal("id_detail_transaksi")))
                                {
                                    DetailTransaksi detail = new DetailTransaksi
                                    {
                                        IdDetailTransaksi = reader.GetInt32(reader.GetOrdinal("id_detail_transaksi")),
                                        IdProduk = reader.GetInt32(reader.GetOrdinal("id_produk")),
                                        // Handle nama produk null (jika produk dihapus)
                                        NamaProduk = reader.IsDBNull(reader.GetOrdinal("nama_produk")) ? "Produk Terhapus" : reader.GetString(reader.GetOrdinal("nama_produk")),
                                        HargaSatuan = reader.GetInt32(reader.GetOrdinal("harga_satuan")),
                                        JumlahBeli = reader.GetInt32(reader.GetOrdinal("jumlah_beli")),
                                        Subtotal = reader.GetInt32(reader.GetOrdinal("jumlah_pembayaran")),
                                        IdTransaksi = idTransaksi
                                    };
                                    currentTransaksi.ListDetail.Add(detail);
                                    // Tidak perlu += TotalBayar lagi karena sudah diambil dari Header DB
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching transaction history:\n{ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return transaksiMap.Values.ToList();
        }

        public Produk? GetProdukById(int idProduk) // Made return type nullable
        {
            Produk? p = null; // Made nullable
            string query = "SELECT id_produk, nama_produk, harga, stok, gambar_produk FROM produk WHERE id_produk = @id";

            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(_dbContext.connStr))
                {
                    conn.Open();
                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idProduk);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                p = new Produk();
                                p.IdProduk = Convert.ToInt32(reader["id_produk"]);
                                p.NamaProduk = reader["nama_produk"].ToString()!;
                                p.Harga = Convert.ToInt32(reader["harga"]);
                                p.Stok = Convert.ToInt32(reader["stok"]);

                                if (reader["gambar_produk"] != DBNull.Value)
                                    p.GambarProduk = (byte[])reader["gambar_produk"];
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal mengambil produk by ID: " + ex.Message);
            }

            return p;
        }

    }
}