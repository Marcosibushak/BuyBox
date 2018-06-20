namespace Ibushak.Productos.Core.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class seextiendecampode250a512 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Caracteristicas");
            AlterColumn("dbo.Caracteristicas", "Caracteristica", c => c.String(nullable: false, maxLength: 512));
            AddPrimaryKey("dbo.Caracteristicas", new[] { "ASIN", "Caracteristica" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.Caracteristicas");
            AlterColumn("dbo.Caracteristicas", "Caracteristica", c => c.String(nullable: false, maxLength: 250));
            AddPrimaryKey("dbo.Caracteristicas", new[] { "ASIN", "Caracteristica" });
        }
    }
}
