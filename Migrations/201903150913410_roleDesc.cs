namespace CascBasic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class roleDesc : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetRoles", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetRoles", "Description");
        }
    }
}
