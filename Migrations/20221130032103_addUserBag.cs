using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class addUserBag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "userName",
                table: "Products",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_userName",
                table: "Products",
                column: "userName");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Users_userName",
                table: "Products",
                column: "userName",
                principalTable: "Users",
                principalColumn: "userName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Users_userName",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_userName",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "userName",
                table: "Products");
        }
    }
}
