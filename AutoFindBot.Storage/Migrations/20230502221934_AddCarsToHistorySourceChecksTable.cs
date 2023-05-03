using Microsoft.EntityFrameworkCore.Migrations;

namespace AutoFindBot.Storage.Migrations
{
    public partial class AddCarsToHistorySourceChecksTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "HistorySourceCheckId",
                table: "Cars",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Cars_HistorySourceCheckId",
                table: "Cars",
                column: "HistorySourceCheckId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_HistorySourceChecks_HistorySourceCheckId",
                table: "Cars",
                column: "HistorySourceCheckId",
                principalTable: "HistorySourceChecks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cars_HistorySourceChecks_HistorySourceCheckId",
                table: "Cars");

            migrationBuilder.DropIndex(
                name: "IX_Cars_HistorySourceCheckId",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "HistorySourceCheckId",
                table: "Cars");
        }
    }
}
