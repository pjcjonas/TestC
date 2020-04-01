namespace DocumentManagerApi.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDocumentFilePath : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Documents", "DocumentFilePath", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Documents", "DocumentFilePath");
        }
    }
}
