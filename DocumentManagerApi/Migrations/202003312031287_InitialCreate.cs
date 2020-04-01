namespace DocumentManagerApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Documents",
                c => new
                    {
                        DocumentId = c.Int(nullable: false, identity: true),
                        DocumentFileName = c.String(nullable: false),
                        AzureFileReference = c.String(),
                        Category = c.String(nullable: false),
                        LastReviewed = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.DocumentId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Documents");
        }
    }
}
