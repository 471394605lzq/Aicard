namespace AiCard.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CardAddIndustry : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cards", "Industry", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Cards", "Industry");
        }
    }
}
