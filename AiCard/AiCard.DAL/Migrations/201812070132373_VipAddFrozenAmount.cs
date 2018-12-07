namespace AiCard.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VipAddFrozenAmount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Vips", "FrozenAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Vips", "FrozenAmount");
        }
    }
}
