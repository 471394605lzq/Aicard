namespace AiCard.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VipAddTotalAmountRank : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Vips", "TotalAmountRank", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Vips", "TotalAmountRank");
        }
    }
}
