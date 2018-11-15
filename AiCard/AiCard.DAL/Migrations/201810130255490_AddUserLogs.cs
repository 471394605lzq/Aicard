namespace AiCard.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserLogs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserLogs",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserID = c.String(),
                        RelationID = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        Remark = c.String(),
                        TargetUserID = c.String(),
                        TargetEnterpriseID = c.Int(),
                        CreateDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UserLogs");
        }
    }
}
