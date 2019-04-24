namespace CascBasic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class menuMore : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MenuItems", "SortOrder", c => c.Long(nullable: false));
            AlterColumn("dbo.MenuItems", "Action", c => c.String(nullable: false));
            AlterColumn("dbo.MenuItems", "Controller", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.MenuItems", "Controller", c => c.String());
            AlterColumn("dbo.MenuItems", "Action", c => c.String());
            DropColumn("dbo.MenuItems", "SortOrder");
        }
    }
}
