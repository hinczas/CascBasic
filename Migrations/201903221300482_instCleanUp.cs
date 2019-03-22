namespace CascBasic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class instCleanUp : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Institutions", "InformChanges");
            DropColumn("dbo.Institutions", "TrumpSafeMinutesShortTerm");
            DropColumn("dbo.Institutions", "TrumpSafeMinutesLongTerm");
            DropColumn("dbo.Institutions", "NoAccountMsg");
            DropColumn("dbo.Institutions", "AcceptInventory");
            DropColumn("dbo.Institutions", "BkAcceptInvInstructions");
            DropColumn("dbo.Institutions", "BkAcceptInvLimitDays");
            DropColumn("dbo.Institutions", "AccInvoiceTypeID");
            DropColumn("dbo.Institutions", "AddrID");
            DropColumn("dbo.Institutions", "ShowRoomState");
            DropColumn("dbo.Institutions", "BkEndDateEmailDays");
            DropColumn("dbo.Institutions", "PrevTicketsOnQuickIssue");
            DropColumn("dbo.Institutions", "PkgeReminderEmailDays");
            DropColumn("dbo.Institutions", "PkgeFromEmail");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Institutions", "PkgeFromEmail", c => c.String());
            AddColumn("dbo.Institutions", "PkgeReminderEmailDays", c => c.Int());
            AddColumn("dbo.Institutions", "PrevTicketsOnQuickIssue", c => c.Int());
            AddColumn("dbo.Institutions", "BkEndDateEmailDays", c => c.Int());
            AddColumn("dbo.Institutions", "ShowRoomState", c => c.Boolean());
            AddColumn("dbo.Institutions", "AddrID", c => c.Int());
            AddColumn("dbo.Institutions", "AccInvoiceTypeID", c => c.Int());
            AddColumn("dbo.Institutions", "BkAcceptInvLimitDays", c => c.Int());
            AddColumn("dbo.Institutions", "BkAcceptInvInstructions", c => c.String());
            AddColumn("dbo.Institutions", "AcceptInventory", c => c.Boolean());
            AddColumn("dbo.Institutions", "NoAccountMsg", c => c.String());
            AddColumn("dbo.Institutions", "TrumpSafeMinutesLongTerm", c => c.Int());
            AddColumn("dbo.Institutions", "TrumpSafeMinutesShortTerm", c => c.Int());
            AddColumn("dbo.Institutions", "InformChanges", c => c.Boolean());
        }
    }
}
