namespace Ibushak.Productos.Core.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreaTablasProducto : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BuyBox",
                c => new
                    {
                        ASIN = c.String(nullable: false, maxLength: 20),
                        Condition = c.String(maxLength: 20),
                        Merchant = c.String(maxLength: 100),
                        Amount = c.String(maxLength: 20),
                        CurrencyCode = c.String(maxLength: 5),
                        FormattedPrice = c.String(maxLength: 100),
                        Availability = c.String(maxLength: 100),
                        AvailabilityType = c.String(maxLength: 50),
                        MinimumHours = c.Int(nullable: false),
                        MaximumHours = c.Int(nullable: false),
                        IsEligibleForSuperSaveShipping = c.Boolean(nullable: false),
                        IseEligibleForPrime = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ASIN)
                .ForeignKey("dbo.Producto", t => t.ASIN)
                .Index(t => t.ASIN);
            
            CreateTable(
                "dbo.Producto",
                c => new
                    {
                        ASIN = c.String(nullable: false, maxLength: 20),
                        Offers = c.String(maxLength: 500),
                        SalesRank = c.Long(nullable: false),
                        SmallImage = c.String(maxLength: 500),
                        MediumImage = c.String(maxLength: 500),
                        LargeImage = c.String(maxLength: 500),
                        Binding = c.String(maxLength: 250),
                        Brand = c.String(maxLength: 250),
                        ClothingSize = c.String(maxLength: 100),
                        Color = c.String(maxLength: 100),
                        Department = c.String(maxLength: 100),
                        EAN = c.String(maxLength: 20),
                        isAdultProduct = c.Boolean(nullable: false),
                        isAutographed = c.Boolean(nullable: false),
                        isMemorabilia = c.Boolean(nullable: false),
                        Label = c.String(maxLength: 100),
                        LegalDisclaimer = c.String(maxLength: 250),
                        Manufacture = c.String(maxLength: 100),
                        Model = c.String(maxLength: 100),
                        MPN = c.String(maxLength: 100),
                        NumberItems = c.Int(nullable: false),
                        PackageQuantity = c.Int(nullable: false),
                        PartNumber = c.Long(nullable: false),
                        ProductGroup = c.String(maxLength: 250),
                        ProdcutTypeName = c.String(maxLength: 250),
                        Publisher = c.String(maxLength: 250),
                        ReleaseDate = c.String(maxLength: 12),
                        Size = c.String(maxLength: 100),
                        Studio = c.String(maxLength: 100),
                        Title = c.String(maxLength: 100),
                        UPC = c.String(maxLength: 15),
                        Amount = c.String(maxLength: 20),
                        CurrencyCode = c.String(maxLength: 5),
                        FormattedPrice = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.ASIN);
            
            CreateTable(
                "dbo.Caracteristicas",
                c => new
                    {
                        ASIN = c.String(nullable: false, maxLength: 20),
                        Caracteristica = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.ASIN, t.Caracteristica })
                .ForeignKey("dbo.Producto", t => t.ASIN, cascadeDelete: true)
                .Index(t => t.ASIN);
            
            CreateTable(
                "dbo.Comentarios",
                c => new
                    {
                        ASIN = c.String(nullable: false, maxLength: 20),
                        Url = c.String(maxLength: 400),
                    })
                .PrimaryKey(t => t.ASIN)
                .ForeignKey("dbo.Producto", t => t.ASIN)
                .Index(t => t.ASIN);
            
            CreateTable(
                "dbo.Dimensiones",
                c => new
                    {
                        ASIN = c.String(nullable: false, maxLength: 20),
                        UnidadMedida = c.String(maxLength: 50),
                        UnidadPeso = c.String(maxLength: 50),
                        Height = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Length = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Weight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Width = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ASIN)
                .ForeignKey("dbo.Producto", t => t.ASIN)
                .Index(t => t.ASIN);
            
            CreateTable(
                "dbo.DimensionesPaquete",
                c => new
                    {
                        ASIN = c.String(nullable: false, maxLength: 20),
                        UnidadMedida = c.String(maxLength: 50),
                        UnidadPeso = c.String(maxLength: 50),
                        Height = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Length = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Weight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Width = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ASIN)
                .ForeignKey("dbo.Producto", t => t.ASIN)
                .Index(t => t.ASIN);
            
            CreateTable(
                "dbo.Resumen",
                c => new
                    {
                        ASIN = c.String(nullable: false, maxLength: 20),
                        LowestPrice = c.String(maxLength: 20),
                        CurrencyCode = c.String(maxLength: 5),
                        FormattedPrice = c.String(maxLength: 100),
                        TotalNew = c.Int(nullable: false),
                        TotalUsed = c.Int(nullable: false),
                        TotalCollectible = c.Int(nullable: false),
                        TotalRefurbished = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ASIN)
                .ForeignKey("dbo.Producto", t => t.ASIN)
                .Index(t => t.ASIN);
            
            CreateTable(
                "dbo.Similares",
                c => new
                    {
                        ASIN = c.String(nullable: false, maxLength: 20),
                        ASINSimilar = c.String(nullable: false, maxLength: 15),
                        Title = c.String(maxLength: 250),
                    })
                .PrimaryKey(t => new { t.ASIN, t.ASINSimilar })
                .ForeignKey("dbo.Producto", t => t.ASIN, cascadeDelete: true)
                .Index(t => t.ASIN);
            
            CreateTable(
                "dbo.UPCs",
                c => new
                    {
                        ASIN = c.String(nullable: false, maxLength: 20),
                        UPC = c.String(nullable: false, maxLength: 15),
                    })
                .PrimaryKey(t => new { t.ASIN, t.UPC })
                .ForeignKey("dbo.Producto", t => t.ASIN, cascadeDelete: true)
                .Index(t => t.ASIN);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UPCs", "ASIN", "dbo.Producto");
            DropForeignKey("dbo.Similares", "ASIN", "dbo.Producto");
            DropForeignKey("dbo.Resumen", "ASIN", "dbo.Producto");
            DropForeignKey("dbo.DimensionesPaquete", "ASIN", "dbo.Producto");
            DropForeignKey("dbo.Dimensiones", "ASIN", "dbo.Producto");
            DropForeignKey("dbo.Comentarios", "ASIN", "dbo.Producto");
            DropForeignKey("dbo.Caracteristicas", "ASIN", "dbo.Producto");
            DropForeignKey("dbo.BuyBox", "ASIN", "dbo.Producto");
            DropIndex("dbo.UPCs", new[] { "ASIN" });
            DropIndex("dbo.Similares", new[] { "ASIN" });
            DropIndex("dbo.Resumen", new[] { "ASIN" });
            DropIndex("dbo.DimensionesPaquete", new[] { "ASIN" });
            DropIndex("dbo.Dimensiones", new[] { "ASIN" });
            DropIndex("dbo.Comentarios", new[] { "ASIN" });
            DropIndex("dbo.Caracteristicas", new[] { "ASIN" });
            DropIndex("dbo.BuyBox", new[] { "ASIN" });
            DropTable("dbo.UPCs");
            DropTable("dbo.Similares");
            DropTable("dbo.Resumen");
            DropTable("dbo.DimensionesPaquete");
            DropTable("dbo.Dimensiones");
            DropTable("dbo.Comentarios");
            DropTable("dbo.Caracteristicas");
            DropTable("dbo.Producto");
            DropTable("dbo.BuyBox");
        }
    }
}
