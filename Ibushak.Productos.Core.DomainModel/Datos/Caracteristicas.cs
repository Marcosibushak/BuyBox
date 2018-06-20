using System;
using Ibushak.Productos.Core.DomainModel.Catologos;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ibushak.Productos.Core.DomainModel.Datos
{
    public class Caracteristicas
    {
        [Key, Column(Order = 1)]
        [ForeignKey("Producto")]
        [StringLength(20)]
        public string ASIN { get; set; }

        [Column(Order = 3)]
        [StringLength(1024)]
        public string Caracteristica { get; set; }

        [Key, Column(Order = 2)]
        public Guid Id { get; set; }

        public Producto Producto { get; set; }

    }
}