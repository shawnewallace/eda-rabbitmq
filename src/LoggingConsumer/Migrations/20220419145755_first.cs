using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eda.loggingConsumer.Migrations
{
    public partial class first : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "log_entries",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoutingKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WhenReceived = table.Column<DateTime>(type: "datetime2", nullable: false),
										CorrelationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
								},
                constraints: table =>
                {
                    table.PrimaryKey("PK_log_entries", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "log_entries",
                schema: "dbo");
        }
    }
}
