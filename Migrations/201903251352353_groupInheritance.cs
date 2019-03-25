namespace CascBasic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class groupInheritance : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetGroups", "ParentId", c => c.String(maxLength: 128));
            CreateIndex("dbo.AspNetGroups", "ParentId");
            AddForeignKey("dbo.AspNetGroups", "ParentId", "dbo.AspNetGroups", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetGroups", "ParentId", "dbo.AspNetGroups");
            DropIndex("dbo.AspNetGroups", new[] { "ParentId" });
            DropColumn("dbo.AspNetGroups", "ParentId");
        }
    }
}
