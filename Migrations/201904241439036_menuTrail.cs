namespace CascBasic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class menuTrail : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MenuItems", "MenuTrail", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MenuItems", "MenuTrail");
        }
    }
}
