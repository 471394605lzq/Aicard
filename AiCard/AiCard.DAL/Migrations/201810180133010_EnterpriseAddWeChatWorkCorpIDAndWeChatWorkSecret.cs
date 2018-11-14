namespace AiCard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EnterpriseAddWeChatWorkCorpIDAndWeChatWorkSecret : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Enterprises", "WeChatWorkCorpid", c => c.String());
            AddColumn("dbo.Enterprises", "WeChatWorkSecret", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Enterprises", "WeChatWorkSecret");
            DropColumn("dbo.Enterprises", "WeChatWorkCorpid");
        }
    }
}
