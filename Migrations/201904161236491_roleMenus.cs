namespace CascBasic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class roleMenus : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.GroupMenus", "GroupId", "dbo.AspNetGroups");
            DropForeignKey("dbo.GroupMenus", "MenuId", "dbo.MenuItems");
            DropIndex("dbo.GroupMenus", new[] { "GroupId" });
            DropIndex("dbo.GroupMenus", new[] { "MenuId" });
            CreateTable(
                "dbo.RoleMenus",
                c => new
                    {
                        RoleId = c.String(nullable: false, maxLength: 128),
                        MenuId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.RoleId, t.MenuId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.MenuItems", t => t.MenuId, cascadeDelete: true)
                .Index(t => t.RoleId)
                .Index(t => t.MenuId);
            
            DropTable("dbo.GroupMenus");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.GroupMenus",
                c => new
                    {
                        GroupId = c.String(nullable: false, maxLength: 128),
                        MenuId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.GroupId, t.MenuId });
            
            DropForeignKey("dbo.RoleMenus", "MenuId", "dbo.MenuItems");
            DropForeignKey("dbo.RoleMenus", "RoleId", "dbo.AspNetRoles");
            DropIndex("dbo.RoleMenus", new[] { "MenuId" });
            DropIndex("dbo.RoleMenus", new[] { "RoleId" });
            DropTable("dbo.RoleMenus");
            CreateIndex("dbo.GroupMenus", "MenuId");
            CreateIndex("dbo.GroupMenus", "GroupId");
            AddForeignKey("dbo.GroupMenus", "MenuId", "dbo.MenuItems", "Id", cascadeDelete: true);
            AddForeignKey("dbo.GroupMenus", "GroupId", "dbo.AspNetGroups", "Id", cascadeDelete: true);
        }
    }
}
