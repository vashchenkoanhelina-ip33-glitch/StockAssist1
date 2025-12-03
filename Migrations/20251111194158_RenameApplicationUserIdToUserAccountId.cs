using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockAssist.Web.Migrations
{
    public partial class RenameApplicationUserIdToUserAccountId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_UserAccountId1",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "UserAccountId1",
                table: "Orders",
                newName: "ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_UserAccountId1",
                table: "Orders",
                newName: "IX_Orders_ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_ApplicationUserId",
                table: "Orders",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_ApplicationUserId",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "ApplicationUserId",
                table: "Orders",
                newName: "UserAccountId1");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_ApplicationUserId",
                table: "Orders",
                newName: "IX_Orders_UserAccountId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_UserAccountId1",
                table: "Orders",
                column: "UserAccountId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
