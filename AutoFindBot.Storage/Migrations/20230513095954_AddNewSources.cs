using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoFindBot.Storage.Migrations
{
    /// <inheritdoc />
    public partial class AddNewSources : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Sources",
                columns: new[] { "Name", "SourceType", "Active" },
                values: new object[] { "TradeDealer", 1, true });
            
            migrationBuilder.InsertData(
                table: "Sources",
                columns: new[] { "Name", "SourceType", "Active" },
                values: new object[] { "KeyAutoProbeg", 2, true });
            
            migrationBuilder.InsertData(
                table: "Sources",
                columns: new[] { "Name", "SourceType", "Active" },
                values: new object[] { "Avito", 3, true });
            
            migrationBuilder.InsertData(
                table: "Sources",
                columns: new[] { "Name", "SourceType", "Active" },
                values: new object[] { "AutoRu", 4, true });
            
            migrationBuilder.InsertData(
                table: "Sources",
                columns: new[] { "Name", "SourceType", "Active" },
                values: new object[] { "Drom", 5, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Sources",
                keyColumn: "Id",
                keyValue: 1);
            
            migrationBuilder.DeleteData(
                table: "Sources",
                keyColumn: "Id",
                keyValue: 2);
            
            migrationBuilder.DeleteData(
                table: "Sources",
                keyColumn: "Id",
                keyValue: 3);
            
            migrationBuilder.DeleteData(
                table: "Sources",
                keyColumn: "Id",
                keyValue: 4);
            
            migrationBuilder.DeleteData(
                table: "Sources",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
