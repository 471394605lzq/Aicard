namespace AiCard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EnterpriseUserCustomerAddSourceAndState : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EnterpriseUserCustomers", "Source", c => c.Int(nullable: false));
            AddColumn("dbo.EnterpriseUserCustomers", "State", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.EnterpriseUserCustomers", "State");
            DropColumn("dbo.EnterpriseUserCustomers", "Source");
        }
    }
}
