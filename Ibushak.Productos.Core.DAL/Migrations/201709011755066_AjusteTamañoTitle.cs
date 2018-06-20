namespace Ibushak.Productos.Core.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AjusteTamañoTitle : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Producto", "Title", c => c.String(maxLength: 250));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Producto", "Title", c => c.String(maxLength: 100));
        }
    }
}
