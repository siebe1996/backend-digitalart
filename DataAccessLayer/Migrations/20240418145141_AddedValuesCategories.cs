using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddedValuesCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Artpieces",
                type: "longtext",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "ImageData",
                table: "Artpieces",
                type: "longtext",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Artpieces",
                type: "longtext",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Categories",
                type: "longtext",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Categories",
                type: "longtext",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Artpieces");

            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "Artpieces");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Artpieces");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Categories");

        }
    }
}
