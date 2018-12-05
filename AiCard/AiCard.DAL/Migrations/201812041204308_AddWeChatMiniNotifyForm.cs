namespace AiCard.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddWeChatMiniNotifyForm : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WeChatMiniNotifyForms",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserID = c.String(),
                        OpenID = c.String(),
                        AppID = c.String(),
                        FormID = c.String(),
                        CreateDateTime = c.DateTime(nullable: false),
                        EndDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.WeChatMiniNotifyForms");
        }
    }
}
