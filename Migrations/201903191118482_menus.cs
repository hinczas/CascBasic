namespace CascBasic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class menus : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MenuItems",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Label = c.String(),
                        Action = c.String(),
                        Controller = c.String(),
                        Url = c.String(),
                        ParentId = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MenuItems", t => t.ParentId)
                .Index(t => t.ParentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MenuItems", "ParentId", "dbo.MenuItems");
            DropIndex("dbo.MenuItems", new[] { "ParentId" });
            DropTable("dbo.MenuItems");
        }
    }
}
