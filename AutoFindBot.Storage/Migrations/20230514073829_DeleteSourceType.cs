using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoFindBot.Storage.Migrations
{
    /// <inheritdoc />
    public partial class DeleteSourceType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SourceType",
                table: "Sources");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SourceType",
                table: "Sources",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
