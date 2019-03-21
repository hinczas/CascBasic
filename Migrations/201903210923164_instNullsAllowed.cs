namespace CascBasic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class instNullsAllowed : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Institutions", "InformChanges", c => c.Boolean());
            AlterColumn("dbo.Institutions", "TrumpSafeMinutesShortTerm", c => c.Int());
            AlterColumn("dbo.Institutions", "TrumpSafeMinutesLongTerm", c => c.Int());
            AlterColumn("dbo.Institutions", "AcceptInventory", c => c.Boolean());
            AlterColumn("dbo.Institutions", "BkAcceptInvLimitDays", c => c.Int());
            AlterColumn("dbo.Institutions", "AccInvoiceTypeID", c => c.Int());
            AlterColumn("dbo.Institutions", "AddrID", c => c.Int());
            AlterColumn("dbo.Institutions", "ShowRoomState", c => c.Boolean());
            AlterColumn("dbo.Institutions", "BkEndDateEmailDays", c => c.Int());
            AlterColumn("dbo.Institutions", "PrevTicketsOnQuickIssue", c => c.Int());
            AlterColumn("dbo.Institutions", "PkgeReminderEmailDays", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Institutions", "PkgeReminderEmailDays", c => c.Int(nullable: false));
            AlterColumn("dbo.Institutions", "PrevTicketsOnQuickIssue", c => c.Int(nullable: false));
            AlterColumn("dbo.Institutions", "BkEndDateEmailDays", c => c.Int(nullable: false));
            AlterColumn("dbo.Institutions", "ShowRoomState", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Institutions", "AddrID", c => c.Int(nullable: false));
            AlterColumn("dbo.Institutions", "AccInvoiceTypeID", c => c.Int(nullable: false));
            AlterColumn("dbo.Institutions", "BkAcceptInvLimitDays", c => c.Int(nullable: false));
            AlterColumn("dbo.Institutions", "AcceptInventory", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Institutions", "TrumpSafeMinutesLongTerm", c => c.Int(nullable: false));
            AlterColumn("dbo.Institutions", "TrumpSafeMinutesShortTerm", c => c.Int(nullable: false));
            AlterColumn("dbo.Institutions", "InformChanges", c => c.Boolean(nullable: false));
        }
    }
}
