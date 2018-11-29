namespace AiCard.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VipAddTotalMonthAmountAndTotalWeekAmount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Vips", "TotalMonthAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Vips", "TotalWeekAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Vips", "TotalWeekAmount");
            DropColumn("dbo.Vips", "TotalMonthAmount");
        }
    }
}
