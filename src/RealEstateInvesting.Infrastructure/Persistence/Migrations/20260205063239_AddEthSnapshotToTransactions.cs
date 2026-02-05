using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateInvesting.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEthSnapshotToTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transactions_PropertyId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_Type",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_UserId",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "Transactions",
                newName: "AmountUsd");

            migrationBuilder.AddColumn<decimal>(
                name: "EthAmountAtExecution",
                table: "Transactions",
                type: "decimal(18,8)",
                precision: 18,
                scale: 8,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "EthUsdRateAtExecution",
                table: "Transactions",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EthAmountAtExecution",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "EthUsdRateAtExecution",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "AmountUsd",
                table: "Transactions",
                newName: "Amount");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_PropertyId",
                table: "Transactions",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_Type",
                table: "Transactions",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserId",
                table: "Transactions",
                column: "UserId");
        }
    }
}
