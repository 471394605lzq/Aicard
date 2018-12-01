namespace AiCard.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddVerificationCode : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.VerificationCodes",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        To = c.String(),
                        Code = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                        IP = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.VerificationCodes");
        }
    }
}
