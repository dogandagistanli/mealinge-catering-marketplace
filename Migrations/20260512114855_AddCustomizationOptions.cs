using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ceng382_25_26_202311031.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomizationOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomizationOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MenuItemId = table.Column<int>(type: "int", nullable: false),
                    GroupName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OptionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OptionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PriceChange = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomizationOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomizationOptions_MenuItems_MenuItemId",
                        column: x => x.MenuItemId,
                        principalTable: "MenuItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_MenuItemId",
                table: "Ratings",
                column: "MenuItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomizationOptions_MenuItemId",
                table: "CustomizationOptions",
                column: "MenuItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_MenuItems_MenuItemId",
                table: "Ratings",
                column: "MenuItemId",
                principalTable: "MenuItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_MenuItems_MenuItemId",
                table: "Ratings");

            migrationBuilder.DropTable(
                name: "CustomizationOptions");

            migrationBuilder.DropIndex(
                name: "IX_Ratings_MenuItemId",
                table: "Ratings");
        }
    }
}
