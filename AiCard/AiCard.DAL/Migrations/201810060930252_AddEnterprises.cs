namespace AiCard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEnterprises : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Enterprises",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Name = c.String(),
                        Logo = c.String(),
                        Info = c.String(),
                        Province = c.String(),
                        City = c.String(),
                        District = c.String(),
                        Address = c.String(),
                        PhoneNumber = c.String(),
                        Email = c.String(),
                        HomePage = c.String(),
                        AdminID = c.String(),
                        AppID = c.String(),
                        CardCount = c.Int(nullable: false),
                        Enable = c.Boolean(nullable: false),
                        RegisterDateTime = c.DateTime(nullable: false),
                        Lat = c.Double(),
                        Lng = c.Double(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Enterprises");
        }
    }
}
