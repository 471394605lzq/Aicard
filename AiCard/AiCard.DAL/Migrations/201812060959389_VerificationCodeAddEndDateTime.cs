namespace AiCard.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VerificationCodeAddEndDateTime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VerificationCodes", "EndDateTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.VerificationCodes", "EndDateTime");
        }
    }
}
