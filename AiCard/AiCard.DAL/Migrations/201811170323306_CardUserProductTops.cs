namespace AiCard.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CardUserProductTops : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CardUserProductTops",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserID = c.String(),
                        CardID = c.Int(nullable: false),
                        ProductID = c.Int(nullable: false),
                        Sort = c.Int(nullable: false),
                        CreateDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CardUserProductTops");
        }
    }
}
