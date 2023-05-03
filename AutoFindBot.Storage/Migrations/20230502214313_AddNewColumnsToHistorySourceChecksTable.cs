using Microsoft.EntityFrameworkCore.Migrations;

namespace AutoFindBot.Storage.Migrations
{
    public partial class AddNewColumnsToHistorySourceChecksTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Data",
                table: "HistorySourceChecks",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Success",
                table: "HistorySourceChecks",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Data",
                table: "HistorySourceChecks");

            migrationBuilder.DropColumn(
                name: "Success",
                table: "HistorySourceChecks");
        }
    }
}
