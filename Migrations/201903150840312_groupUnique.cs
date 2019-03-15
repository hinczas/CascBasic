namespace CascBasic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class groupUnique : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Groups", "Name", c => c.String(maxLength: 64));
            CreateIndex("dbo.Groups", "Name", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Groups", new[] { "Name" });
            AlterColumn("dbo.Groups", "Name", c => c.String());
        }
    }
}
