namespace AiCard.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserSpeechsAndUserSpeechTypes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserSpeeches",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Content = c.String(),
                        UserID = c.String(),
                        Sort = c.Int(nullable: false),
                        TypeID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.UserSpeechTypes", t => t.TypeID, cascadeDelete: true)
                .Index(t => t.TypeID);
            
            CreateTable(
                "dbo.UserSpeechTypes",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Sort = c.Int(nullable: false),
                        UserID = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserSpeeches", "TypeID", "dbo.UserSpeechTypes");
            DropIndex("dbo.UserSpeeches", new[] { "TypeID" });
            DropTable("dbo.UserSpeechTypes");
            DropTable("dbo.UserSpeeches");
        }
    }
}
