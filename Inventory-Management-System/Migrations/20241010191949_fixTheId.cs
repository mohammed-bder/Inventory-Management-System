using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class fixTheId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EmployeeSuppliers",
                table: "EmployeeSuppliers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmployeeSuppliers",
                table: "EmployeeSuppliers",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSuppliers_EmployeeID",
                table: "EmployeeSuppliers",
                column: "EmployeeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EmployeeSuppliers",
                table: "EmployeeSuppliers");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeSuppliers_EmployeeID",
                table: "EmployeeSuppliers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmployeeSuppliers",
                table: "EmployeeSuppliers",
                columns: new[] { "EmployeeID", "SupplierID" });
        }
    }
}
