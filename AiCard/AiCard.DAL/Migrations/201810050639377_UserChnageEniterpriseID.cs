namespace AiCard.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserChnageEniterpriseID : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RoleGroups", "EnterpriseID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RoleGroups", "EnterpriseID", c => c.Int());
        }
    }
}
