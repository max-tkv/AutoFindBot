using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace AutoFindBot.Storage.Migrations
{
    public partial class AddHistorySourceChecksTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HistorySourceChecks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserFilterId = table.Column<long>(type: "bigint", nullable: false),
                    Source = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "current_timestamp"),
                    UpdatedDateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "current_timestamp")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorySourceChecks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorySourceChecks_UserFilters_UserFilterId",
                        column: x => x.UserFilterId,
                        principalTable: "UserFilters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HistorySourceChecks_UserFilterId",
                table: "HistorySourceChecks",
                column: "UserFilterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistorySourceChecks");
        }
    }
}
