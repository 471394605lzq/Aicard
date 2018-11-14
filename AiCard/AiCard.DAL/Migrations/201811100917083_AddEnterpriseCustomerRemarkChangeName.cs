namespace AiCard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEnterpriseCustomerRemarkChangeName : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.EnterpriseCustomerRemarks", newName: "EnterpriseUserCustomers");
            AddColumn("dbo.EnterpriseUserCustomers", "CreateDateTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.EnterpriseUserCustomers", "CreateDateTime");
            RenameTable(name: "dbo.EnterpriseUserCustomers", newName: "EnterpriseCustomerRemarks");
        }
    }
}
