using ChochoNest.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ChochoNest.Controller
{
    public class TransaksiContext
    {
        private readonly string _connectionString;

        public TransaksiContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        // --- 1. FITUR LIHAT RIWAYAT ---
        public List<TransaksiModel> GetRiwayatTransaksi(int? userId = null)
        {
            var transaksis = new List<TransaksiModel>();
            string query = @"
                SELECT 
                    t.id_transaksi, t.tanggal_transaksi, t.total_bayar,              
                    t.id_user, u.username, 
                    t.id_metode_pembayaran, mp.nama_metode_pembayaran,  
                    dt.id_detail_transaksi, dt.id_produk, p.nama_produk, 
                    p.harga, dt.jumlah_beli, dt.jumlah_pembayaran        
                FROM transaksi t
                JOIN ""user"" u ON t.id_user = u.id_user
                LEFT JOIN metode_pembayaran mp ON t.id_metode_pembayaran = mp.id_metode_pembayaran
                LEFT JOIN detail_transaksi dt ON t.id_transaksi = dt.id_transaksi
                LEFT JOIN produk p ON dt.id_produk = p.id_produk
                {WHERE_CLAUSE}
                ORDER BY t.tanggal_transaksi DESC, t.id_transaksi DESC";

            query = query.Replace("{WHERE_CLAUSE}", userId.HasValue ? "WHERE t.id_user = @id_user" : "");

            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        if (userId.HasValue) cmd.Parameters.AddWithValue("@id_user", userId.Value);
                        using (var reader = cmd.ExecuteReader())
                        {
                            var transaksiMap = new Dictionary<int, TransaksiModel>();
                            while (reader.Read())
                            {
                                int idTransaksi = Convert.ToInt32(reader["id_transaksi"]);
                                if (!transaksiMap.ContainsKey(idTransaksi))
                                {
                                    var trx = new TransaksiModel
                                    {
                                        IdTransaksi = idTransaksi,
                                        TanggalTransaksi = Convert.ToDateTime(reader["tanggal_transaksi"]),
                                        TotalBayar = Convert.ToInt32(reader["total_bayar"]),
                                        IdUser = Convert.ToInt32(reader["id_user"]),
                                        Username = reader["username"].ToString(),
                                        IdMetodePembayaran = reader["id_metode_pembayaran"] != DBNull.Value ? Convert.ToInt32(reader["id_metode_pembayaran"]) : 0,
                                        MetodePembayaran = reader["nama_metode_pembayaran"] != DBNull.Value ? reader["nama_metode_pembayaran"].ToString() : "-",
                                        ListDetail = new List<DetailTransaksi>()
                                    };
                                    transaksiMap.Add(idTransaksi, trx);
                                    transaksis.Add(trx);
                                }

                                if (reader["id_detail_transaksi"] != DBNull.Value)
                                {
                                    var detail = new DetailTransaksi
                                    {
                                        IdDetailTransaksi = Convert.ToInt32(reader["id_detail_transaksi"]),
                                        IdTransaksi = idTransaksi,
                                        IdProduk = Convert.ToInt32(reader["id_produk"]),
                                        NamaProduk = reader["nama_produk"] != DBNull.Value ? reader["nama_produk"].ToString() : "Produk Dihapus",
                                        HargaSatuan = reader["harga"] != DBNull.Value ? Convert.ToInt32(reader["harga"]) : 0,
                                        JumlahBeli = Convert.ToInt32(reader["jumlah_beli"]),
                                        Subtotal = Convert.ToInt32(reader["jumlah_pembayaran"])
                                    };
                                    transaksiMap[idTransaksi].ListDetail.Add(detail);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Error Load Data: " + ex.Message); }
            return transaksis;
        }

        // --- 2. FITUR SIMPAN TRANSAKSI (YANG TADI MISSING) ---
        public void SimpanTransaksi(TransaksiModel transaksiBaru)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        // A. Insert Header Transaksi
                        string queryHeader = "INSERT INTO transaksi (tanggal_transaksi, total_bayar, id_user, id_metode_pembayaran) VALUES (@tgl, @total, @idUser, @idMetode) RETURNING id_transaksi";
                        int idTransaksiBaru;
                        using (var cmd = new NpgsqlCommand(queryHeader, conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@tgl", DateTime.Now);
                            cmd.Parameters.AddWithValue("@total", transaksiBaru.TotalBayar);
                            cmd.Parameters.AddWithValue("@idUser", transaksiBaru.IdUser);
                            cmd.Parameters.AddWithValue("@idMetode", transaksiBaru.IdMetodePembayaran == 0 ? 1 : transaksiBaru.IdMetodePembayaran);
                            idTransaksiBaru = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        // B. Insert Detail & Update Stok
                        foreach (var detail in transaksiBaru.ListDetail)
                        {
                            // Insert Detail
                            string queryDetail = "INSERT INTO detail_transaksi (id_transaksi, id_produk, jumlah_beli, jumlah_pembayaran) VALUES (@idTrans, @idProd, @qty, @subtotal)";
                            using (var cmdDetail = new NpgsqlCommand(queryDetail, conn, tran))
                            {
                                cmdDetail.Parameters.AddWithValue("@idTrans", idTransaksiBaru);
                                cmdDetail.Parameters.AddWithValue("@idProd", detail.IdProduk);
                                cmdDetail.Parameters.AddWithValue("@qty", detail.JumlahBeli);
                                cmdDetail.Parameters.AddWithValue("@subtotal", detail.Subtotal);
                                cmdDetail.ExecuteNonQuery();
                            }

                            // Update Stok
                            string queryStok = "UPDATE produk SET stok = stok - @qty WHERE id_produk = @idProd";
                            using (var cmdStok = new NpgsqlCommand(queryStok, conn, tran))
                            {
                                cmdStok.Parameters.AddWithValue("@qty", detail.JumlahBeli);
                                cmdStok.Parameters.AddWithValue("@idProd", detail.IdProduk);
                                if (cmdStok.ExecuteNonQuery() == 0) throw new Exception($"Produk ID {detail.IdProduk} tidak ditemukan.");
                            }
                        }
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        throw; // Lempar error biar ditangkap form
                    }
                }
            }
        }
    }
} 