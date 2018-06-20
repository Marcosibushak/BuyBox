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
    public class Comentarios
    {
        [Key]
        [ForeignKey("Producto")]
        [StringLength(20)]
        public string ASIN { get; set; }

        [StringLength(400)]
        public string Url { get; set; }

        public Producto Producto { get; set; }
    }
}
