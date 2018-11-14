namespace AiCard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ArticleAddUpdateDateTimeAndTypeAndShare : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Articles", "Type", c => c.Int(nullable: false));
            AddColumn("dbo.Articles", "UpdateDateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.Articles", "Share", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Articles", "Share");
            DropColumn("dbo.Articles", "UpdateDateTime");
            DropColumn("dbo.Articles", "Type");
        }
    }
}
