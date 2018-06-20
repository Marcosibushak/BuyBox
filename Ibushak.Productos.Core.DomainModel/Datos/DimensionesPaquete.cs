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
    public class DimensionesPaquete
    {
        [Key]
        [ForeignKey("Producto")]
        [StringLength(20)]
        public string ASIN { get; set; }

        [StringLength(50)]
        public string UnidadMedida { get; set; }

        [StringLength(50)]
        public string UnidadPeso { get; set; }

        public decimal Height { get; set; }

        public decimal Length { get; set; }

        public decimal Weight { get; set; }

        public decimal Width { get; set; }

        public Producto Producto { get; set; }
    }
}
