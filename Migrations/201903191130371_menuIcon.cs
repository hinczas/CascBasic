namespace CascBasic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class menuIcon : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MenuItems", "FlatIconName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MenuItems", "FlatIconName");
        }
    }
}
