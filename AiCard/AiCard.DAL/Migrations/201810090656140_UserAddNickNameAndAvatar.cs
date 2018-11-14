namespace AiCard.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserAddNickNameAndAvatar : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "WeChatID", c => c.String());
            AddColumn("dbo.AspNetUsers", "NickName", c => c.String());
            AddColumn("dbo.AspNetUsers", "Avatar", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Avatar");
            DropColumn("dbo.AspNetUsers", "NickName");
            DropColumn("dbo.AspNetUsers", "WeChatID");
        }
    }
}
