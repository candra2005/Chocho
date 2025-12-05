using ChochoNest.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms; // jika bukan WinForms, ganti logging/exception handling

namespace ChochoNest.Controller
{
    /// <summary>
    /// Versi "satu class" yang berisi logic untuk mengambil riwayat transaksi
    /// beserta model-detailnya. Konstruktor menerima connection string PostgreSQL.
    /// </summary>
    public class TransaksiContextFull
    {
        private readonly string _connectionString;

        public TransaksiContextFull(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        /// <summary>
        /// Ambil riwayat transaksi. Jika userId null => ambil semua transaksi.
        /// </summary>
        /// <param name="userId">opsional, filter transaksi berdasarkan id_user</param>
        /// <returns>List TransaksiModel</returns>
        public List<TransaksiModel> GetRiwayatTransaksi(int? userId = null)
        {
            var transaksis = new List<TransaksiModel>();

            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();

                    string query = @"
SELECT
    t.id_transaksi,
    t.tanggal_transaksi,
    t.id_user,
    u.username,
    t.id_metode_pembayaran,
    mp.nama_metode_pembayaran AS metode_pembayaran_nama,
    dt.id_detail_transaksi,
    dt.id_produk,
    p.nama_produk,
    p.harga AS harga_satuan,
    dt.jumlah_beli,
    dt.jumlah_pembayaran
FROM transaksi t
JOIN ""user"" u ON t.id_user = u.id_user
LEFT JOIN metode_pembayaran mp ON t.id_metode_pembayaran = mp.id_metode_pembayaran
LEFT JOIN detail_transaksi dt ON t.id_transaksi = dt.id_transaksi
LEFT JOIN produk p ON dt.id_produk = p.id_produk
{WHERE_CLAUSE}
ORDER BY t.tanggal_transaksi DESC, t.id_transaksi DESC;
";

                    string whereClause = userId.HasValue ? "WHERE t.id_user = @id_user" : string.Empty;
                    query = query.Replace("{WHERE_CLAUSE}", whereClause);

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        if (userId.HasValue)
                            cmd.Parameters.AddWithValue("@id_user", userId.Value);

                        using (var reader = cmd.ExecuteReader())
                        {
                            // map id_transaksi -> TransaksiModel
                            var transaksiMap = new Dictionary<int, TransaksiModel>();

                            // Pre-get ordinals (optional optimization)
                            int ordIdTransaksi = -1;
                            int ordTanggal = -1;
                            int ordIdUser = -1;
                            int ordUsername = -1;
                            int ordIdMetode = -1;
                            int ordMetodeNama = -1;
                            int ordIdDetail = -1;
                            int ordIdProduk = -1;
                            int ordNamaProduk = -1;
                            int ordHargaSatuan = -1;
                            int ordJumlahBeli = -1;
                            int ordJumlahPembayaran = -1;

                            // Try to assign ordinals if columns exist
                            try
                            {
                                ordIdTransaksi = reader.GetOrdinal("id_transaksi");
                                ordTanggal = reader.GetOrdinal("tanggal_transaksi");
                                ordIdUser = reader.GetOrdinal("id_user");
                                ordUsername = reader.GetOrdinal("username");
                                ordIdMetode = reader.GetOrdinal("id_metode_pembayaran");
                                ordMetodeNama = reader.GetOrdinal("metode_pembayaran_nama");
                                ordIdDetail = reader.GetOrdinal("id_detail_transaksi");
                                ordIdProduk = reader.GetOrdinal("id_produk");
                                ordNamaProduk = reader.GetOrdinal("nama_produk");
                                ordHargaSatuan = reader.GetOrdinal("harga_satuan");
                                ordJumlahBeli = reader.GetOrdinal("jumlah_beli");
                                ordJumlahPembayaran = reader.GetOrdinal("jumlah_pembayaran");
                            }
                            catch
                            {
                                // Jika struktur kolom berbeda, akan terlihat saat membaca tiap baris.
                            }

                            while (reader.Read())
                            {
                                // baca id_transaksi — pastikan kolom ada
                                if (ordIdTransaksi < 0 || reader.IsDBNull(ordIdTransaksi))
                                    continue; // baris tidak valid tanpa id_transaksi

                                int idTransaksi = reader.GetInt32(ordIdTransaksi);

                                // Buat TransaksiModel baru jika belum ada
                                if (!transaksiMap.ContainsKey(idTransaksi))
                                {
                                    var transaksi = new TransaksiModel
                                    {
                                        IdTransaksi = idTransaksi,
                                        TanggalTransaksi = (ordTanggal >= 0 && !reader.IsDBNull(ordTanggal))
                                                            ? reader.GetDateTime(ordTanggal)
                                                            : DateTime.MinValue,
                                        IdUser = (ordIdUser >= 0 && !reader.IsDBNull(ordIdUser))
                                                            ? reader.GetInt32(ordIdUser)
                                                            : 0,
                                        Username = (ordUsername >= 0 && !reader.IsDBNull(ordUsername))
                                                            ? reader.GetString(ordUsername)
                                                            : null,
                                        IdMetodePembayaran = (ordIdMetode >= 0 && !reader.IsDBNull(ordIdMetode))
                                                            ? reader.GetInt32(ordIdMetode)
                                                            : 0,
                                        MetodePembayaran = (ordMetodeNama >= 0 && !reader.IsDBNull(ordMetodeNama))
                                                            ? reader.GetString(ordMetodeNama)
                                                            : null,
                                        ListDetail = new List<DetailTransaksi>(),
                                        TotalBayar = 0
                                    };

                                    transaksiMap[idTransaksi] = transaksi;
                                    transaksis.Add(transaksi);
                                }

                                var current = transaksiMap[idTransaksi];

                                // Jika tidak ada detail (LEFT JOIN memberikan null pada id_detail_transaksi), skip pembuatan detail
                                if (ordIdDetail < 0 || reader.IsDBNull(ordIdDetail))
                                    continue;

                                var detail = new DetailTransaksi
                                {
                                    IdDetailTransaksi = reader.IsDBNull(ordIdDetail) ? 0 : reader.GetInt32(ordIdDetail),
                                    IdTransaksi = idTransaksi,
                                    IdProduk = (ordIdProduk >= 0 && !reader.IsDBNull(ordIdProduk)) ? reader.GetInt32(ordIdProduk) : 0,
                                    NamaProduk = (ordNamaProduk >= 0 && !reader.IsDBNull(ordNamaProduk)) ? reader.GetString(ordNamaProduk) : "Produk telah dihapus",
                                    HargaSatuan = (ordHargaSatuan >= 0 && !reader.IsDBNull(ordHargaSatuan)) ? reader.GetInt32(ordHargaSatuan) : 0,
                                    JumlahBeli = (ordJumlahBeli >= 0 && !reader.IsDBNull(ordJumlahBeli)) ? reader.GetInt32(ordJumlahBeli) : 0,
                                    Subtotal = (ordJumlahPembayaran >= 0 && !reader.IsDBNull(ordJumlahPembayaran)) ? reader.GetInt32(ordJumlahPembayaran) : 0
                                };

                                current.ListDetail.Add(detail);
                                current.TotalBayar += detail.Subtotal;
                            } // while reader.Read()
                        } // using reader
                    } // using cmd
                } // using conn
            }
            catch (Exception ex)
            {
                // Tampilkan exception lengkap sementara untuk debugging.
                // Ganti dengan logger di production.
                MessageBox.Show("Error fetching transaction history:\n" + ex.ToString());
            }

            return transaksis;
        }
    }
}