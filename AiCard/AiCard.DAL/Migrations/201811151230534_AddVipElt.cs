namespace AiCard.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddVipElt : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Channel = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        State = c.Int(nullable: false),
                        PayCode = c.String(),
                        UserID = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ReceivableAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreateDateTime = c.DateTime(nullable: false),
                        PayDateTime = c.DateTime(),
                        PayInput = c.String(),
                        PayResult = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.VipAmountLogs",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserID = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Before = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Type = c.Int(nullable: false),
                        SourceUserID = c.String(),
                        VipID = c.Int(),
                        CreateDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.VipForwardOrders",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        PayCode = c.String(),
                        UserID = c.String(),
                        Type = c.Int(nullable: false),
                        FromAccount = c.String(),
                        ToAccount = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ReceivableAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        State = c.Int(nullable: false),
                        Remark = c.String(),
                        PayResult = c.String(),
                        CreateDateTime = c.DateTime(nullable: false),
                        PayDateTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.VipRelationships",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserID = c.String(),
                        VipID = c.Int(nullable: false),
                        ParentUserID = c.String(),
                        ParentID = c.Int(nullable: false),
                        CreateDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Vips",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserID = c.String(),
                        CardID = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        VipChild2ndCount = c.Int(nullable: false),
                        VipChild3rdCount = c.Int(nullable: false),
                        FreeChildCount = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        State = c.Int(nullable: false),
                        Code = c.String(),
                        CreateDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Vips");
            DropTable("dbo.VipRelationships");
            DropTable("dbo.VipForwardOrders");
            DropTable("dbo.VipAmountLogs");
            DropTable("dbo.Orders");
        }
    }
}
