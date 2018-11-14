namespace AiCard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserProductTop : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserProductTops",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserID = c.String(),
                        ProductID = c.Int(nullable: false),
                        CreateDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.Cards", "Poster", c => c.String());
            AddColumn("dbo.UserLogs", "Total", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserLogs", "Total");
            DropColumn("dbo.Cards", "Poster");
            DropTable("dbo.UserProductTops");
        }
    }
}
