namespace AiCard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RoleAddTypeGroupDesc : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetRoles", "Type", c => c.Int());
            AddColumn("dbo.AspNetRoles", "Group", c => c.String());
            AddColumn("dbo.AspNetRoles", "Description", c => c.String());
            AddColumn("dbo.AspNetRoles", "Discriminator", c => c.String(nullable: false, maxLength: 128));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetRoles", "Discriminator");
            DropColumn("dbo.AspNetRoles", "Description");
            DropColumn("dbo.AspNetRoles", "Group");
            DropColumn("dbo.AspNetRoles", "Type");
        }
    }
}
