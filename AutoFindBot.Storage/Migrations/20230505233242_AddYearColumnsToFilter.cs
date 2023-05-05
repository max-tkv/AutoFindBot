using Microsoft.EntityFrameworkCore.Migrations;

namespace AutoFindBot.Storage.Migrations
{
    public partial class AddYearColumnsToFilter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "YearMax",
                table: "UserFilters",
                type: "integer",
                nullable: false,
                defaultValue: -1);

            migrationBuilder.AddColumn<int>(
                name: "YearMin",
                table: "UserFilters",
                type: "integer",
                nullable: false,
                defaultValue: -1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YearMax",
                table: "UserFilters");

            migrationBuilder.DropColumn(
                name: "YearMin",
                table: "UserFilters");
        }
    }
}
