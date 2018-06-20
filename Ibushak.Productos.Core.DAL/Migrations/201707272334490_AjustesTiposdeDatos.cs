namespace Ibushak.Productos.Core.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AjustesTiposdeDatos : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.BuyBox", "ASIN", "dbo.Producto");
            DropPrimaryKey("dbo.BuyBox");
            AlterColumn("dbo.BuyBox", "Condition", c => c.String(nullable: false, maxLength: 20));
            AlterColumn("dbo.Producto", "PartNumber", c => c.String(maxLength: 100));
            AddPrimaryKey("dbo.BuyBox", new[] { "ASIN", "Condition" });
            AddForeignKey("dbo.BuyBox", "ASIN", "dbo.Producto", "ASIN", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BuyBox", "ASIN", "dbo.Producto");
            DropPrimaryKey("dbo.BuyBox");
            AlterColumn("dbo.Producto", "PartNumber", c => c.Long(nullable: false));
            AlterColumn("dbo.BuyBox", "Condition", c => c.String(maxLength: 20));
            AddPrimaryKey("dbo.BuyBox", "ASIN");
            AddForeignKey("dbo.BuyBox", "ASIN", "dbo.Producto", "ASIN");
        }
    }
}
