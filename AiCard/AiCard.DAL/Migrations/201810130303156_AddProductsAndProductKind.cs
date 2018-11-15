namespace AiCard.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProductsAndProductKind : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductKinds",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        EnterpriseID = c.Int(nullable: false),
                        Sort = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        KindID = c.Int(nullable: false),
                        EnterpriseID = c.Int(nullable: false),
                        Name = c.String(),
                        Count = c.Int(nullable: false),
                        TotalSales = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Release = c.Boolean(nullable: false),
                        Images = c.String(),
                        Info = c.String(),
                        Sort = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.ProductKinds", t => t.KindID, cascadeDelete: true)
                .Index(t => t.KindID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Products", "KindID", "dbo.ProductKinds");
            DropIndex("dbo.Products", new[] { "KindID" });
            DropTable("dbo.Products");
            DropTable("dbo.ProductKinds");
        }
    }
}
