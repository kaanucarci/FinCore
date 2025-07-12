using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinCore.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddTotalAmountToBudget : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "Budgets",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "Budgets");
        }
    }
}
