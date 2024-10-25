using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class makeAlertHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StartAlerts_ProductId",
                table: "StartAlerts");

            migrationBuilder.CreateIndex(
                name: "IX_StartAlerts_ProductId",
                table: "StartAlerts",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StartAlerts_ProductId",
                table: "StartAlerts");

            migrationBuilder.CreateIndex(
                name: "IX_StartAlerts_ProductId",
                table: "StartAlerts",
                column: "ProductId",
                unique: true);
        }
    }
}
