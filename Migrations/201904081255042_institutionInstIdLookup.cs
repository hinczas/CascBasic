namespace CascBasic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class institutionInstIdLookup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Institutions", "InstId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Institutions", "InstId");
        }
    }
}
