namespace AiCard.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteOwnerIDAndCardID : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.EnterpriseCustomers", "CardID");
            DropColumn("dbo.EnterpriseCustomers", "OwnerID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.EnterpriseCustomers", "OwnerID", c => c.String());
            AddColumn("dbo.EnterpriseCustomers", "CardID", c => c.Int(nullable: false));
        }
    }
}
