namespace AiCard.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ArticleDeleteEnableAddState : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Articles", "State", c => c.Int(nullable: false));
            DropColumn("dbo.Articles", "Enable");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Articles", "Enable", c => c.Boolean(nullable: false));
            DropColumn("dbo.Articles", "State");
        }
    }
}
