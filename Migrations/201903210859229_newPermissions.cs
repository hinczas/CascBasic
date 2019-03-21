namespace CascBasic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newPermissions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Institutions",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        ZEmail = c.String(),
                        WelcomeMsg = c.String(),
                        Campus = c.String(),
                        CollegeName = c.String(),
                        CollegePhone = c.String(),
                        CollegeFax = c.String(),
                        CollegeVATnumber = c.String(),
                        CollegeCrest = c.Binary(),
                        CollegeCharityNumber = c.String(),
                        InformChanges = c.Boolean(nullable: false),
                        TrumpSafeMinutesShortTerm = c.Int(nullable: false),
                        TrumpSafeMinutesLongTerm = c.Int(nullable: false),
                        NoAccountMsg = c.String(),
                        ContactName = c.String(),
                        ContactEmail = c.String(),
                        AcceptInventory = c.Boolean(nullable: false),
                        BkAcceptInvInstructions = c.String(),
                        BkAcceptInvLimitDays = c.Int(nullable: false),
                        AccInvoiceTypeID = c.Int(nullable: false),
                        AddrID = c.Int(nullable: false),
                        ShowRoomState = c.Boolean(nullable: false),
                        BkEndDateEmailDays = c.Int(nullable: false),
                        PrevTicketsOnQuickIssue = c.Int(nullable: false),
                        PkgeReminderEmailDays = c.Int(nullable: false),
                        PkgeFromEmail = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Permissions",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(maxLength: 64),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.RolePermissions",
                c => new
                    {
                        RoleId = c.String(nullable: false, maxLength: 128),
                        PermId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.RoleId, t.PermId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.Permissions", t => t.PermId, cascadeDelete: true)
                .Index(t => t.RoleId)
                .Index(t => t.PermId);
            
            CreateTable(
                "dbo.GroupPermissions",
                c => new
                    {
                        GroupId = c.String(nullable: false, maxLength: 128),
                        PermId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.GroupId, t.PermId })
                .ForeignKey("dbo.AspNetGroups", t => t.GroupId, cascadeDelete: true)
                .ForeignKey("dbo.Permissions", t => t.PermId, cascadeDelete: true)
                .Index(t => t.GroupId)
                .Index(t => t.PermId);
            
            AddColumn("dbo.AspNetGroups", "InstId", c => c.Long(nullable: false));
            CreateIndex("dbo.AspNetGroups", "InstId");
            AddForeignKey("dbo.AspNetGroups", "InstId", "dbo.Institutions", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GroupPermissions", "PermId", "dbo.Permissions");
            DropForeignKey("dbo.GroupPermissions", "GroupId", "dbo.AspNetGroups");
            DropForeignKey("dbo.RolePermissions", "PermId", "dbo.Permissions");
            DropForeignKey("dbo.RolePermissions", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.AspNetGroups", "InstId", "dbo.Institutions");
            DropIndex("dbo.GroupPermissions", new[] { "PermId" });
            DropIndex("dbo.GroupPermissions", new[] { "GroupId" });
            DropIndex("dbo.RolePermissions", new[] { "PermId" });
            DropIndex("dbo.RolePermissions", new[] { "RoleId" });
            DropIndex("dbo.Permissions", new[] { "Name" });
            DropIndex("dbo.AspNetGroups", new[] { "InstId" });
            DropColumn("dbo.AspNetGroups", "InstId");
            DropTable("dbo.GroupPermissions");
            DropTable("dbo.RolePermissions");
            DropTable("dbo.Permissions");
            DropTable("dbo.Institutions");
        }
    }
}
