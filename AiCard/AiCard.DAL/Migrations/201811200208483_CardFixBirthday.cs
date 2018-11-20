namespace AiCard.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CardFixBirthday : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Cards", "Birthday", c => c.DateTime());
            DropColumn("dbo.CardTabs", "Birthday");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CardTabs", "Birthday", c => c.DateTime());
            DropColumn("dbo.Cards", "Birthday");
        }
    }
}
