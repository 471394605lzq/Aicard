namespace AiCard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCardTabs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CardTabs",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CardID = c.Int(nullable: false),
                        Style = c.Int(nullable: false),
                        Count = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Cards", t => t.CardID, cascadeDelete: true)
                .Index(t => t.CardID);
            
            AddColumn("dbo.Cards", "WeChatMiniQrCode", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CardTabs", "CardID", "dbo.Cards");
            DropIndex("dbo.CardTabs", new[] { "CardID" });
            DropColumn("dbo.Cards", "WeChatMiniQrCode");
            DropTable("dbo.CardTabs");
        }
    }
}
