using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockAssist.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddSavedCardToApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CardBrand",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CardExpiry",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CardLast4",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CardBrand",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CardExpiry",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CardLast4",
                table: "AspNetUsers");
        }
    }
}
