using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChochoNest.Models
{
    public class Produk
    {
        public int IdProduk { get; set; }
        public string? NamaProduk { get; set; }
        public int Harga { get; set; }
        public int Stok { get; set; }
        public byte[]? GambarProduk { get; set; }
        public bool IsDeleted { get; set; }
    }
}
