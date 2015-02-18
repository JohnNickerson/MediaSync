namespace AssimilationSoftware.MediaSync.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LinkParticipantsToProfiles : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FileHeaders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RelativePath = c.String(maxLength: 4000),
                        FileName = c.String(maxLength: 4000),
                        FileSize = c.Long(nullable: false),
                        ContentsHash = c.String(maxLength: 4000),
                        FileIndex_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.FileIndexes", t => t.FileIndex_Id)
                .Index(t => t.FileIndex_Id);
            
            CreateTable(
                "dbo.FileIndexes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TimeStamp = c.DateTime(nullable: false),
                        Participant_Id = c.Int(),
                        Profile_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProfileParticipants", t => t.Participant_Id)
                .ForeignKey("dbo.SyncProfiles", t => t.Profile_Id)
                .Index(t => t.Participant_Id)
                .Index(t => t.Profile_Id);
            
            CreateTable(
                "dbo.ProfileParticipants",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LocalPath = c.String(maxLength: 4000),
                        SharedPath = c.String(maxLength: 4000),
                        MachineName = c.String(maxLength: 4000),
                        Contributor = c.Boolean(nullable: false),
                        Consumer = c.Boolean(nullable: false),
                        Profile_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SyncProfiles", t => t.Profile_Id)
                .Index(t => t.Profile_Id);
            
            CreateTable(
                "dbo.SyncProfiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FileIndexes", "Profile_Id", "dbo.SyncProfiles");
            DropForeignKey("dbo.FileIndexes", "Participant_Id", "dbo.ProfileParticipants");
            DropForeignKey("dbo.ProfileParticipants", "Profile_Id", "dbo.SyncProfiles");
            DropForeignKey("dbo.FileHeaders", "FileIndex_Id", "dbo.FileIndexes");
            DropIndex("dbo.ProfileParticipants", new[] { "Profile_Id" });
            DropIndex("dbo.FileIndexes", new[] { "Profile_Id" });
            DropIndex("dbo.FileIndexes", new[] { "Participant_Id" });
            DropIndex("dbo.FileHeaders", new[] { "FileIndex_Id" });
            DropTable("dbo.SyncProfiles");
            DropTable("dbo.ProfileParticipants");
            DropTable("dbo.FileIndexes");
            DropTable("dbo.FileHeaders");
        }
    }
}
