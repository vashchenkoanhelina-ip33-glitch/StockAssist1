using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockAssist.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddQuantityAndStoragePeriodToOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_ApplicationUserId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ApplicationUserId",
                table: "Orders");

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

            migrationBuilder.RenameColumn(
                name: "ApplicationUserId",
                table: "Orders",
                newName: "StorageTo");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Orders",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "StorageFrom",
                table: "Orders",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "StorageFrom",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "StorageTo",
                table: "Orders",
                newName: "ApplicationUserId");

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

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ApplicationUserId",
                table: "Orders",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_ApplicationUserId",
                table: "Orders",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
