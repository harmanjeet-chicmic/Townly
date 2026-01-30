using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateInvesting.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMonthlyIncomeToUserPortfolioSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MonthlyIncome",
                table: "UserPortfolioSnapshots",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MonthlyIncome",
                table: "UserPortfolioSnapshots");
        }
    }
}
