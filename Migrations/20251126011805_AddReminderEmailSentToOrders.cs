using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockAssist.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddReminderEmailSentToOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ReminderEmailSent",
                table: "Orders",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReminderEmailSent",
                table: "Orders");
        }
    }
}
