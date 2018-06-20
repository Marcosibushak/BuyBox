using Ibushak.Productos.Core.DomainModel.Catologos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ibushak.Productos.Core.DomainModel.Datos
{
    public class Similares
    {
        [Key, Column(Order = 1)]
        [ForeignKey("Producto")]
        [StringLength(20)]
        public string ASIN { get; set; }

        [Key, Column(Order = 2)]
        [StringLength(15)]
        public string ASINSimilar { get; set; }

        [StringLength(512)]
        public string Title { get; set; }

        public Producto Producto { get; set; }
    }
}
