namespace AiCard.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCustomerTabAndCustomerTabGroup : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CustomerTabGroups",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Style = c.Int(nullable: false),
                        Sort = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.CustomerTabs",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        GroupID = c.Int(nullable: false),
                        Name = c.String(),
                        Sort = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.CustomerTabGroups", t => t.GroupID, cascadeDelete: true)
                .Index(t => t.GroupID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CustomerTabs", "GroupID", "dbo.CustomerTabGroups");
            DropIndex("dbo.CustomerTabs", new[] { "GroupID" });
            DropTable("dbo.CustomerTabs");
            DropTable("dbo.CustomerTabGroups");
        }
    }
}
