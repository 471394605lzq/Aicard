namespace AiCard.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CardAddSortAndAddEnterpriseCustomerTab : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EnterpriseCustomerTabs",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        CustomerID = c.Int(nullable: false),
                        OwnerID = c.String(),
                        Name = c.String(),
                        Style = c.Int(nullable: false),
                        CreateDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.EnterpriseCustomers", t => t.CustomerID, cascadeDelete: true)
                .Index(t => t.CustomerID);
            
            AddColumn("dbo.Cards", "Sort", c => c.Int(nullable: false));
            DropColumn("dbo.EnterpriseCustomerRemarks", "Tabs");
        }
        
        public override void Down()
        {
            AddColumn("dbo.EnterpriseCustomerRemarks", "Tabs", c => c.String());
            DropForeignKey("dbo.EnterpriseCustomerTabs", "CustomerID", "dbo.EnterpriseCustomers");
            DropIndex("dbo.EnterpriseCustomerTabs", new[] { "CustomerID" });
            DropColumn("dbo.Cards", "Sort");
            DropTable("dbo.EnterpriseCustomerTabs");
        }
    }
}
