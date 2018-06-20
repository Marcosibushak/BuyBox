namespace Ibushak.Productos.Core.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AjusteCampoActualizacion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Producto", "Actualizacion", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Producto", "Actualizacion");
        }
    }
}
