namespace SampleStoreLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class inital : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Samples",
                c => new
                    {
                        SampleID = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 100),
                        Genre = c.String(maxLength: 100),
                        Mp4Blob = c.String(maxLength: 1024),
                        SampleMp4Blob = c.String(maxLength: 1024),
                        SampleMp4URL = c.String(maxLength: 1024),
                        CreatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.SampleID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Samples");
        }
    }
}
