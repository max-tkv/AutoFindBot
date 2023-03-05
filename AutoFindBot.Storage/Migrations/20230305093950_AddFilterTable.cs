using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AutoFindBot.Storage.Migrations
{
    public partial class AddFilterTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "UserFilterId",
                table: "Cars",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "UserFilters",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PriceMin = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PriceMax = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "current_timestamp"),
                    UpdatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "current_timestamp")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFilters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserFilters_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cars_UserFilterId",
                table: "Cars",
                column: "UserFilterId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFilters_Title",
                table: "UserFilters",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_UserFilters_UserId",
                table: "UserFilters",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_UserFilters_UserFilterId",
                table: "Cars",
                column: "UserFilterId",
                principalTable: "UserFilters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cars_UserFilters_UserFilterId",
                table: "Cars");

            migrationBuilder.DropTable(
                name: "UserFilters");

            migrationBuilder.DropIndex(
                name: "IX_Cars_UserFilterId",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "UserFilterId",
                table: "Cars");
        }
    }
}
