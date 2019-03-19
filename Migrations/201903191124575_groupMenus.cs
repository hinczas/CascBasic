namespace CascBasic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class groupMenus : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GroupMenus",
                c => new
                    {
                        GroupId = c.String(nullable: false, maxLength: 128),
                        MenuId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.GroupId, t.MenuId })
                .ForeignKey("dbo.AspNetGroups", t => t.GroupId, cascadeDelete: true)
                .ForeignKey("dbo.MenuItems", t => t.MenuId, cascadeDelete: true)
                .Index(t => t.GroupId)
                .Index(t => t.MenuId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GroupMenus", "MenuId", "dbo.MenuItems");
            DropForeignKey("dbo.GroupMenus", "GroupId", "dbo.AspNetGroups");
            DropIndex("dbo.GroupMenus", new[] { "MenuId" });
            DropIndex("dbo.GroupMenus", new[] { "GroupId" });
            DropTable("dbo.GroupMenus");
        }
    }
}
