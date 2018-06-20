namespace Ibushak.Productos.Core.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreacionUsuarios : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Usuario",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Psw = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Usuario");
        }
    }
}
