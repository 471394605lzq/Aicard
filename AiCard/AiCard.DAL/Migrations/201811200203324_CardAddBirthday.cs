namespace AiCard.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CardAddBirthday : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CardTabs", "Birthday", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CardTabs", "Birthday");
        }
    }
}
