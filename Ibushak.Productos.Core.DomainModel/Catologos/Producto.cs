using Ibushak.Productos.Core.DomainModel.Datos;
using Ibushak.Productos.Core.DomainModel.Ofertas;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Ibushak.Productos.Core.DomainModel.Catologos
{
    public class Producto
    {
        [Key]
        [StringLength(20)]
        public string ASIN { get; set; }

        [StringLength(500)]
        public string Offers { get; set; }

        public long SalesRank { get; set; }

        [StringLength(500)]
        public string SmallImage { get; set; }

        [StringLength(500)]
        public string MediumImage { get; set; }

        [StringLength(500)]
        public string LargeImage { get; set; }

        [StringLength(250)]
        public string Binding { get; set; }

        [StringLength(250)]
        public string Brand { get; set; }

        [StringLength(100)]
        public string ClothingSize { get; set; }

        [StringLength(512)]
        public string Color { get; set; }

        [StringLength(100)]
        public string Department { get; set; }

        [StringLength(20)]
        public string EAN { get; set; }

        public bool isAdultProduct { get; set; }

        public bool isAutographed { get; set; }

        public bool isMemorabilia { get; set; }

        [StringLength(100)]
        public string Label { get; set; }

        [StringLength(4096)]
        public string LegalDisclaimer { get; set; }

        [StringLength(100)]
        public string Manufacture { get; set; }

        [StringLength(100)]
        public string Model { get; set; }

        [StringLength(100)]
        public string MPN { get; set; }

        public int NumberItems { get; set; }

        public int PackageQuantity { get; set; }

        [StringLength(100)]
        public string PartNumber { get; set; }

        [StringLength(250)]
        public string ProductGroup { get; set; }

        [StringLength(250)]
        public string ProdcutTypeName { get; set; }

        [StringLength(250)]
        public string Publisher { get; set; }

        [StringLength(12)]
        public string ReleaseDate { get; set; }

        [StringLength(100)]
        public string Size { get; set; }

        [StringLength(100)]
        public string Studio { get; set; }

        [StringLength(512)]
        public string Title { get; set; }

        [StringLength(15)]
        public string UPC { get; set; }

        [StringLength(20)]
        public string Amount { get; set; }

        [StringLength(5)]
        public string CurrencyCode { get; set; }

        [StringLength(100)]
        public string FormattedPrice { get; set; }

        public bool Actualizacion { get; set; }
        [StringLength(100)]
        public string Netsuite { get; set; }

        public bool IsUpdated { get; set; }

        public Resumen Resumen { get; set; }

        public List<BuyBox> BuyBox { get; set; }

        public Comentarios Comentarios { get; set; }

        public Dimensiones Dimensiones { get; set; }

        public DimensionesPaquete DimensionesPaquete { get; set; }

        public List<Caracteristicas> Caracteristicas { get; set; }

        public List<Similares> Similares { get; set; }

        public List<UPCs> UPCs { get; set; }
    }
}
