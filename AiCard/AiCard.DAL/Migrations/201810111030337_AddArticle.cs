namespace AiCard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddArticle : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ArticleComments",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ArticleID = c.Int(nullable: false),
                        Content = c.String(),
                        UserID = c.String(),
                        CreateDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Articles", t => t.ArticleID, cascadeDelete: true)
                .Index(t => t.ArticleID);
            
            CreateTable(
                "dbo.Articles",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        CreateDateTime = c.DateTime(nullable: false),
                        EnterpriseID = c.Int(),
                        UserID = c.String(),
                        Content = c.String(),
                        Images = c.String(),
                        Video = c.String(),
                        Like = c.Int(nullable: false),
                        Comment = c.Int(nullable: false),
                        Enable = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ArticleComments", "ArticleID", "dbo.Articles");
            DropIndex("dbo.ArticleComments", new[] { "ArticleID" });
            DropTable("dbo.Articles");
            DropTable("dbo.ArticleComments");
        }
    }
}
