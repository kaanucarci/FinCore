using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinCore.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddExpenseTypeToExpense : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExpenseType",
                table: "Expenses",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpenseType",
                table: "Expenses");
        }
    }
}
