namespace AiCard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CardAddViewAndLike : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cards", "Like", c => c.Int(nullable: false));
            AddColumn("dbo.Cards", "View", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Cards", "View");
            DropColumn("dbo.Cards", "Like");
        }
    }
}
