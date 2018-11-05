namespace AiCard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CustomerTabGroupAddEnterpriseID : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CustomerTabGroups", "EnterpriseID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CustomerTabGroups", "EnterpriseID");
        }
    }
}
