
using System;
using System.Collections.Generic;

namespace ChochoNest.Models
{
    public class TransaksiModel
    {
        public int IdTransaksi { get; set; }
        public DateTime TanggalTransaksi { get; set; }
        public int IdUser { get; set; }
        public string? Username { get; set; }
        public int IdMetodePembayaran { get; set; }
        public string? MetodePembayaran { get; set; }
        public int TotalBayar { get; set; }
        public List<DetailTransaksi> ListDetail { get; set; } = new List<DetailTransaksi>();
    }

    public class DetailTransaksi
    {
        public int IdDetailTransaksi { get; set; }
        public int IdTransaksi { get; set; }
        public int IdProduk { get; set; }
        public string? NamaProduk { get; set; }
        public int HargaSatuan { get; set; }
        public int JumlahBeli { get; set; }
        public int Subtotal { get; set; }
    }

    // If you need Keranjang model, keep it here inside the same namespace.
    internal class Keranjang
    {
        public int Id { get; set; }
        public int ProdukId { get; set; } = 0;
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal PriceTotal { get; set; }
        public int QuantityTotal { get; set; }
    }
}