namespace Logger.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class what : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.LogEntries", newName: "LogEntryEntities");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.LogEntryEntities", newName: "LogEntries");
        }
    }
}
