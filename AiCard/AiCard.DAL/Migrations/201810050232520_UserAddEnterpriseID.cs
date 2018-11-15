namespace AiCard.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserAddEnterpriseID : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "EnterpriseID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "EnterpriseID");
        }
    }
}
