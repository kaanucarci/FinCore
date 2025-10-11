using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinCore.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddUserResetCodeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserResetCodes_Users_UserId1",
                table: "UserResetCodes");

            migrationBuilder.DropIndex(
                name: "IX_UserResetCodes_UserId1",
                table: "UserResetCodes");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "UserResetCodes");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "UserResetCodes",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "IX_UserResetCodes_UserId",
                table: "UserResetCodes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserResetCodes_Users_UserId",
                table: "UserResetCodes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserResetCodes_Users_UserId",
                table: "UserResetCodes");

            migrationBuilder.DropIndex(
                name: "IX_UserResetCodes_UserId",
                table: "UserResetCodes");

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "UserResetCodes",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
