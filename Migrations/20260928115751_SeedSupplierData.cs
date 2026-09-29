using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProductInventoryAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeedSupplierData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Suppliers",
                columns: new[] { "Id", "ContactEmail", "Name", "Phone" },
                values: new object[,]
                {
                    { 1, "abc@example.com", "ABC Electronics", "9876543210" },
                    { 2, "tech@example.com", "Tech Supplies", "9876543211" },
                    { 3, "global@example.com", "Global Components", "9876543212" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
