using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinCore.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddIsVerifiedToUserResetCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "UserResetCodes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "UserResetCodes");
        }

    }
}
