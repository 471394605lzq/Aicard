namespace AiCard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddHomePageModulars : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HomePageModulars",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        EnterpriseID = c.Int(nullable: false),
                        Title = c.String(),
                        Type = c.Int(nullable: false),
                        Sort = c.Int(nullable: false),
                        Content = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.HomePageModulars");
        }
    }
}
