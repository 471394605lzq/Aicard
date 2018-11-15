namespace AiCard.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEnterpriseCustomerAndEnterpriseCustomerRemark : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EnterpriseCustomerRemarks",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        CustomerID = c.Int(nullable: false),
                        OwnerID = c.String(),
                        Remark = c.String(),
                        Tabs = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.EnterpriseCustomers", t => t.CustomerID, cascadeDelete: true)
                .Index(t => t.CustomerID);
            
            CreateTable(
                "dbo.EnterpriseCustomers",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        EnterpriseID = c.Int(nullable: false),
                        UserID = c.String(),
                        CardID = c.Int(nullable: false),
                        OwnerID = c.String(),
                        RealName = c.String(),
                        Gender = c.Int(nullable: false),
                        Mobile = c.String(),
                        Position = c.String(),
                        Company = c.String(),
                        Birthday = c.DateTime(),
                        Email = c.String(),
                        Province = c.String(),
                        City = c.String(),
                        District = c.String(),
                        Address = c.String(),
                        Tabs = c.String(),
                        Lat = c.Double(),
                        Lng = c.Double(),
                        Log = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EnterpriseCustomerRemarks", "CustomerID", "dbo.EnterpriseCustomers");
            DropIndex("dbo.EnterpriseCustomerRemarks", new[] { "CustomerID" });
            DropTable("dbo.EnterpriseCustomers");
            DropTable("dbo.EnterpriseCustomerRemarks");
        }
    }
}
