namespace Logger.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class organization : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.LogEntryEntities", newName: "log_entries");
            MoveTable(name: "dbo.log_entries", newSchema: "app");
        }
        
        public override void Down()
        {
            MoveTable(name: "app.log_entries", newSchema: "dbo");
            RenameTable(name: "dbo.log_entries", newName: "LogEntryEntities");
        }
    }
}
