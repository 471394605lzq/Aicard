namespace AiCard.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCardPersonal : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CardPersonals",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Enterprise = c.String(),
                        UserID = c.String(maxLength: 128),
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
                        WeChatMiniQrCode = c.String(),
                        Poster = c.String(),
                        Industry = c.String(),
                        Like = c.Int(nullable: false),
                        View = c.Int(nullable: false),
                        Birthday = c.DateTime(),
                        Sort = c.Int(nullable: false),
                        Province = c.String(),
                        City = c.String(),
                        District = c.String(),
                        Lat = c.Double(),
                        Lng = c.Double(),
                        Address = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AspNetUsers", t => t.UserID)
                .Index(t => t.UserID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CardPersonals", "UserID", "dbo.AspNetUsers");
            DropIndex("dbo.CardPersonals", new[] { "UserID" });
            DropTable("dbo.CardPersonals");
        }
    }
}
