using Microsoft.EntityFrameworkCore.Migrations;

namespace BugTracker.Data.Migrations
{
    public partial class ThirdMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Bug",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bug_ApplicationUserId",
                table: "Bug",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bug_AspNetUsers_ApplicationUserId",
                table: "Bug",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bug_AspNetUsers_ApplicationUserId",
                table: "Bug");

            migrationBuilder.DropIndex(
                name: "IX_Bug_ApplicationUserId",
                table: "Bug");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Bug");
        }
    }
}
