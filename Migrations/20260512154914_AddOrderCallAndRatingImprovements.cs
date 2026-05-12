using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ceng382_25_26_202311031.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderCallAndRatingImprovements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderItemId",
                table: "Ratings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CatererId",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_OrderItemId",
                table: "Ratings",
                column: "OrderItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_OrderItems_OrderItemId",
                table: "Ratings",
                column: "OrderItemId",
                principalTable: "OrderItems",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_OrderItems_OrderItemId",
                table: "Ratings");

            migrationBuilder.DropIndex(
                name: "IX_Ratings_OrderItemId",
                table: "Ratings");

            migrationBuilder.DropColumn(
                name: "OrderItemId",
                table: "Ratings");

            migrationBuilder.DropColumn(
                name: "CatererId",
                table: "OrderItems");
        }
    }
}
