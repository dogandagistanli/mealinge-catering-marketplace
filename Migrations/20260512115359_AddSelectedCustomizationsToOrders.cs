using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ceng382_25_26_202311031.Migrations
{
    /// <inheritdoc />
    public partial class AddSelectedCustomizationsToOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CustomizationPrice",
                table: "OrderItems",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "SelectedCustomizations",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomizationPrice",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "SelectedCustomizations",
                table: "OrderItems");
        }
    }
}
