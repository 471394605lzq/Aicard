namespace AiCard.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserAddOpenIDs : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "OpenIDs", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "OpenIDs");
        }
    }
}
