namespace Ibushak.Productos.Core.DAL
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using Ibushak.Productos.Core.DomainModel.Catologos;
    using System.Data.Entity.ModelConfiguration.Conventions;
    using Ibushak.Productos.Core.DomainModel.Ofertas;
    using Ibushak.Productos.Core.DomainModel.Datos;
    using Ibushak.Productos.Core.DomainModel.Seguridad;

    public partial class IbushakProductosContext : DbContext
    {
        public IbushakProductosContext()
            : base("name=IbushakProductosContext")
        {
        }

        public DbSet<ASIN> ASIN { get; set; }
        public DbSet<UPC> UPC { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Resumen> Resumen { get; set; }
        public DbSet<BuyBox> BuyBox { get; set; }
        public DbSet<Caracteristicas> Caracteristicas { get; set; }
        public DbSet<Comentarios> Comentarios { get; set; }
        public DbSet<Dimensiones> Dimensiones { get; set; }
        public DbSet<DimensionesPaquete> DimensionesPaquete { get; set; }
        public DbSet<Similares> Similares { get; set; }
        public DbSet<UPCs> UPCs { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
