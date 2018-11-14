namespace AiCard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCard : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Cards",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        EnterpriseID = c.Int(),
                        UserID = c.String(maxLength: 128),
                        WeChatEID = c.String(),
                        Name = c.String(),
                        Avatar = c.String(),
                        PhoneNumber = c.String(),
                        Email = c.String(),
                        WeChatCode = c.String(),
                        Mobile = c.String(),
                        Enable = c.Boolean(nullable: false),
                        Position = c.String(),
                        Gender = c.Int(nullable: false),
                        Remark = c.String(),
                        Info = c.String(),
                        Voice = c.String(),
                        Video = c.String(),
                        Images = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Enterprises", t => t.EnterpriseID)
                .ForeignKey("dbo.AspNetUsers", t => t.UserID)
                .Index(t => t.EnterpriseID)
                .Index(t => t.UserID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Cards", "UserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.Cards", "EnterpriseID", "dbo.Enterprises");
            DropIndex("dbo.Cards", new[] { "UserID" });
            DropIndex("dbo.Cards", new[] { "EnterpriseID" });
            DropTable("dbo.Cards");
        }
    }
}
