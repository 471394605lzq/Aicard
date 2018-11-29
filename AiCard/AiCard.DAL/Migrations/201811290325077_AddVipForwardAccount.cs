namespace AiCard.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddVipForwardAccount : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.VipForwardAccounts",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ForwardType = c.Int(nullable: false),
                        ForwardAccount = c.String(),
                        ForwardName = c.String(),
                        Bank = c.String(),
                        BankCode = c.String(),
                        CreateDateTime = c.DateTime(nullable: false),
                        CerCode = c.String(),
                        PhoneNumber = c.String(),
                        VipID = c.Int(nullable: false),
                        UserID = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.VipForwardAccounts");
        }
    }
}
