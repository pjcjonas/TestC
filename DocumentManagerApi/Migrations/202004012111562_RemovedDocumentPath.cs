namespace DocumentManagerApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedDocumentPath : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Documents", "DocumentFilePath");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Documents", "DocumentFilePath", c => c.String(nullable: false));
        }
    }
}
