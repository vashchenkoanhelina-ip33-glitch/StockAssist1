using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockAssist.Web.Migrations
{
    public partial class AddStorageFieldsToOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "Payments",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "StorageMonths",
                table: "Orders",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "StoragePricePerMonth",
                table: "Orders",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<double>(
                name: "VolumeM3",
                table: "Orders",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "WeightKg",
                table: "Orders",
                type: "REAL",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WarehouseProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WarehouseId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WarehouseProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WarehouseProducts_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseProducts_ProductId",
                table: "WarehouseProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseProducts_WarehouseId",
                table: "WarehouseProducts",
                column: "WarehouseId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WarehouseProducts");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "StorageMonths",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "StoragePricePerMonth",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "VolumeM3",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "WeightKg",
                table: "Orders");
        }
    }
}
