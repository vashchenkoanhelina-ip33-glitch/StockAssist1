using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockAssist.Web.Migrations
{
    public partial class AddNotesToOrders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Orders",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WarehouseId",
                table: "Orders",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_WarehouseId",
                table: "Orders",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Warehouses_WarehouseId",
                table: "Orders",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Warehouses_WarehouseId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_WarehouseId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "Orders");
        }
    }
}
