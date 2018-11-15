namespace AiCard.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CustomerTabChangeSortType : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CustomerTabs", "Sort", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CustomerTabs", "Sort", c => c.String());
        }
    }
}
