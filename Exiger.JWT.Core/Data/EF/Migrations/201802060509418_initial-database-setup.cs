namespace Exiger.JWT.Core.Data.EF.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initialdatabasesetup : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ClientAccount",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        ClientName = c.String(nullable: false, maxLength: 100),
                        TokenTimeOut = c.Int(nullable: false),
                        OpenTokensLimit = c.Int(nullable: false),
                        WhitelistIPs = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Active = c.Boolean(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                        Client_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClientAccount", t => t.Client_id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex")
                .Index(t => t.Client_id);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.ClientIPAddresses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IPAddress4 = c.String(nullable: false),
                        LoginUser_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.LoginUser_Id)
                .Index(t => t.LoginUser_Id);
            
            CreateTable(
                "dbo.ClientConnections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TokenStartDateTimeUTC = c.DateTime(nullable: false),
                        TokenEndDateTimeUTC = c.DateTime(nullable: false),
                        ConnectionAuditLog_Id = c.Int(nullable: false),
                        ConnectionClientAccount_id = c.Int(nullable: false),
                        LoginUser_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AuditActivityLog", t => t.ConnectionAuditLog_Id)
                .ForeignKey("dbo.ClientAccount", t => t.ConnectionClientAccount_id)
                .ForeignKey("dbo.AspNetUsers", t => t.LoginUser_Id)
                .Index(t => t.ConnectionAuditLog_Id)
                .Index(t => t.ConnectionClientAccount_id)
                .Index(t => t.LoginUser_Id);
            
            CreateTable(
                "dbo.AuditActivityLog",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SenderUserID = c.String(nullable: false),
                        SenderIP = c.String(nullable: false),
                        SenderRequestURL = c.String(nullable: false),
                        InsightEmail = c.String(maxLength: 320),
                        AuthenticationToken = c.String(),
                        TokenStartDateTimeUTC = c.DateTime(),
                        TokenEndDateTimeUTC = c.DateTime(),
                        ErrorMessage = c.String(),
                        AuditDateTime = c.DateTime(nullable: false),
                        LoginUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.LoginUser_Id)
                .Index(t => t.LoginUser_Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.IdentityUserLogin",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        LoginProvider = c.String(),
                        ProviderKey = c.String(),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.IdentityRole",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.IdentityUserRole",
                c => new
                    {
                        RoleId = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                        IdentityRole_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => new { t.RoleId, t.UserId })
                .ForeignKey("dbo.IdentityRole", t => t.IdentityRole_Id)
                .Index(t => t.IdentityRole_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.IdentityUserRole", "IdentityRole_Id", "dbo.IdentityRole");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.ClientConnections", "LoginUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.ClientConnections", "ConnectionClientAccount_id", "dbo.ClientAccount");
            DropForeignKey("dbo.ClientConnections", "ConnectionAuditLog_Id", "dbo.AuditActivityLog");
            DropForeignKey("dbo.AuditActivityLog", "LoginUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.ClientIPAddresses", "LoginUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "Client_id", "dbo.ClientAccount");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.IdentityUserRole", new[] { "IdentityRole_Id" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AuditActivityLog", new[] { "LoginUser_Id" });
            DropIndex("dbo.ClientConnections", new[] { "LoginUser_Id" });
            DropIndex("dbo.ClientConnections", new[] { "ConnectionClientAccount_id" });
            DropIndex("dbo.ClientConnections", new[] { "ConnectionAuditLog_Id" });
            DropIndex("dbo.ClientIPAddresses", new[] { "LoginUser_Id" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", new[] { "Client_id" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropTable("dbo.IdentityUserRole");
            DropTable("dbo.IdentityRole");
            DropTable("dbo.IdentityUserLogin");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AuditActivityLog");
            DropTable("dbo.ClientConnections");
            DropTable("dbo.ClientIPAddresses");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.ClientAccount");
        }
    }
}
