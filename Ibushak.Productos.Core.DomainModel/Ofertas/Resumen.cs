using Ibushak.Productos.Core.DomainModel.Catologos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ibushak.Productos.Core.DomainModel.Ofertas
{
    public class Resumen
    {
        [Key]
        [ForeignKey("Producto")]
        [StringLength(20)]
        public string ASIN { get; set; }

        [StringLength(20)]
        public string LowestPrice { get; set; }

        [StringLength(5)]
        public string CurrencyCode { get; set; }

        [StringLength(100)]
        public string FormattedPrice { get; set; }

        public int TotalNew { get; set; }

        public int TotalUsed { get; set; }

        public int TotalCollectible { get; set; }

        public int TotalRefurbished { get; set; }

        public Producto Producto { get; set; }
    }
}
