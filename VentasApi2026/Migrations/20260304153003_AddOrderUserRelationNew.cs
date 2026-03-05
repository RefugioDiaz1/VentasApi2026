using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VentasApi2026.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderUserRelationNew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CreatedByUserId",
                table: "Orders",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Users_CreatedByUserId",
                table: "Orders",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Users_CreatedByUserId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CreatedByUserId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Orders");
        }
    }
}
