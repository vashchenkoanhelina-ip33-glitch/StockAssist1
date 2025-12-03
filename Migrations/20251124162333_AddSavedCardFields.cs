using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockAssist.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddSavedCardFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Products_ProductId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseProducts_Products_ProductId",
                table: "WarehouseProducts");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropIndex(
                name: "IX_WarehouseProducts_ProductId",
                table: "WarehouseProducts");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "OrderItems");

            migrationBuilder.AddColumn<string>(
                name: "BillingAddress",
                table: "Payments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingCity",
                table: "Payments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingCountry",
                table: "Payments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingZip",
                table: "Payments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CardBrand",
                table: "Payments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CardCvv",
                table: "Payments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CardExpiry",
                table: "Payments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CardNumber",
                table: "Payments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Payments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Payments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MonthlyStoragePrice",
                table: "Payments",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "OrderNumber",
                table: "Payments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethodRaw",
                table: "Payments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Payments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SaveCard",
                table: "Payments",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SavedCardBrand",
                table: "Payments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SavedCardExpiry",
                table: "Payments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SavedCardLast4",
                table: "Payments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StorageMonths",
                table: "Payments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "Payments",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "SavedCardBrand",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SavedCardExpiry",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SavedCardLast4",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillingAddress",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "BillingCity",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "BillingCountry",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "BillingZip",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "CardBrand",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "CardCvv",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "CardExpiry",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "CardNumber",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "MonthlyStoragePrice",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "OrderNumber",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PaymentMethodRaw",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "SaveCard",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "SavedCardBrand",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "SavedCardExpiry",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "SavedCardLast4",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "StorageMonths",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "SavedCardBrand",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SavedCardExpiry",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SavedCardLast4",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "OrderItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", nullable: false),
                    Stock = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseProducts_ProductId",
                table: "WarehouseProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Products_ProductId",
                table: "OrderItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseProducts_Products_ProductId",
                table: "WarehouseProducts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
