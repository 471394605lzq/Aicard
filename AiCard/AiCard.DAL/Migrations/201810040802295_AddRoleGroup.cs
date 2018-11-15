namespace AiCard.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRoleGroup : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RoleGroups",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Roles = c.String(),
                        EnterpriseID = c.Int(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RoleGroups");
        }
    }
}
