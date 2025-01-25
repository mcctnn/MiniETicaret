using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniETicaret.ShoppingCarts.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class mgasads : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShoppingCartId",
                table: "ShoppingCarts",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ShoppingCarts",
                newName: "ShoppingCartId");
        }
    }
}
