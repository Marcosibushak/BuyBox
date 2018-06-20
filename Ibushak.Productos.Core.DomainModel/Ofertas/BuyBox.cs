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
    public class BuyBox
    {
        [Key, Column(Order = 1)]
        [ForeignKey("Producto")]
        [StringLength(20)]
        public string ASIN { get; set; }

        [Key, Column(Order = 2)]
        [StringLength(20)]
        public string Condition { get; set; }

        [StringLength(100)]
        public string Merchant { get; set; }

        [StringLength(20)]
        public string Amount { get; set; }

        [StringLength(5)]
        public string CurrencyCode { get; set; }

        [StringLength(100)]
        public string FormattedPrice { get; set; }

        [StringLength(256)]
        public string Availability { get; set; }

        [StringLength(50)]
        public string AvailabilityType { get; set; }

        public int MinimumHours { get; set; }

        public int MaximumHours { get; set; }

        public bool IsEligibleForSuperSaveShipping { get; set; }

        public bool IseEligibleForPrime { get; set; }

        public Producto Producto { get; set; }

    }
}
