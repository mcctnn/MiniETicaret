using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniETicaret.ShoppingCarts.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class mig321 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Stock",
                table: "ShoppingCarts",
                newName: "Quantity");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "ShoppingCarts",
                newName: "Stock");
        }
    }
}
