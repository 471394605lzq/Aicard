namespace AiCard.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VipTotalWeekAmountRankAndTotalMonthAmountRank : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Vips", "TotalWeekAmountRank", c => c.Int(nullable: false));
            AddColumn("dbo.Vips", "TotalMonthAmountRank", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Vips", "TotalMonthAmountRank");
            DropColumn("dbo.Vips", "TotalWeekAmountRank");
        }
    }
}
