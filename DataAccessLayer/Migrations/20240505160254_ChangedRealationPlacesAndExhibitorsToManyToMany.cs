using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class ChangedRealationPlacesAndExhibitorsToManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exhibitors_Places_PlaceId",
                table: "Exhibitors");

            migrationBuilder.DropIndex(
                name: "IX_Exhibitors_PlaceId",
                table: "Exhibitors");

            migrationBuilder.DropColumn(
                name: "PlaceId",
                table: "Exhibitors");

            migrationBuilder.CreateTable(
                name: "ExhibitorPlaces",
                columns: table => new
                {
                    ExhibitorId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    PlaceId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExhibitorPlaces", x => new { x.ExhibitorId, x.PlaceId });
                    table.ForeignKey(
                        name: "FK_ExhibitorPlaces_Exhibitors_ExhibitorId",
                        column: x => x.ExhibitorId,
                        principalTable: "Exhibitors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExhibitorPlaces_Places_PlaceId",
                        column: x => x.PlaceId,
                        principalTable: "Places",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ExhibitorPlaces_PlaceId",
                table: "ExhibitorPlaces",
                column: "PlaceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExhibitorPlaces");

            migrationBuilder.AddColumn<Guid>(
                name: "PlaceId",
                table: "Exhibitors",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Exhibitors_PlaceId",
                table: "Exhibitors",
                column: "PlaceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exhibitors_Places_PlaceId",
                table: "Exhibitors",
                column: "PlaceId",
                principalTable: "Places",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
