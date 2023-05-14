using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoFindBot.Storage.Migrations
{
    /// <inheritdoc />
    public partial class AddYoulaSource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Sources",
                columns: new[] { "Name", "Active" },
                values: new object[] { "Youla", false });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Sources",
                keyColumn: "Id",
                keyValue: 6);
        }
    }
}
