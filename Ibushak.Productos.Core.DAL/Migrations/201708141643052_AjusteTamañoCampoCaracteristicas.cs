namespace Ibushak.Productos.Core.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AjusteTamañoCampoCaracteristicas : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Caracteristicas");
            AlterColumn("dbo.Caracteristicas", "Caracteristica", c => c.String(nullable: false, maxLength: 250));
            AddPrimaryKey("dbo.Caracteristicas", new[] { "ASIN", "Caracteristica" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.Caracteristicas");
            AlterColumn("dbo.Caracteristicas", "Caracteristica", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.Caracteristicas", new[] { "ASIN", "Caracteristica" });
        }
    }
}
