using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AutoFindBot.Storage.Migrations
{
    public partial class AddCarsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cars",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Source = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<int>(type: "int", nullable: false),
                    Vin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "current_timestamp"),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "current_timestamp")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cars", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cars_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cars_Title_Price_Year_Source",
                table: "Cars",
                columns: new[] { "Title", "Price", "Year", "Source" });

            migrationBuilder.CreateIndex(
                name: "IX_Cars_UserId",
                table: "Cars",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cars");
        }
    }
}
