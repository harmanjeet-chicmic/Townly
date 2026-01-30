using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateInvesting.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePropertyFinancialModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Properties_OwnerUserId",
                table: "Properties");

            migrationBuilder.DropIndex(
                name: "IX_Properties_Status",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "ExpectedAnnualYield",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "MonthlyRent",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "PricePerShare",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "TotalValueUsd",
                table: "Properties");

            migrationBuilder.RenameColumn(
                name: "TotalShares",
                table: "Properties",
                newName: "TotalUnits");

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "Properties",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<decimal>(
                name: "AnnualYieldPercent",
                table: "Properties",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ApprovedValuation",
                table: "Properties",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "InitialValuation",
                table: "Properties",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnnualYieldPercent",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "ApprovedValuation",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "InitialValuation",
                table: "Properties");

            migrationBuilder.RenameColumn(
                name: "TotalUnits",
                table: "Properties",
                newName: "TotalShares");

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "Properties",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300);

            migrationBuilder.AddColumn<decimal>(
                name: "ExpectedAnnualYield",
                table: "Properties",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MonthlyRent",
                table: "Properties",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerShare",
                table: "Properties",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "Properties",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalValueUsd",
                table: "Properties",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Properties_OwnerUserId",
                table: "Properties",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_Status",
                table: "Properties",
                column: "Status");
        }
    }
}
