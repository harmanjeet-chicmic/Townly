using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateInvesting.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAnalyticsSnapshots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PropertyAnalyticsSnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SnapshotAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SharesSold = table.Column<int>(type: "int", nullable: false),
                    TotalInvested = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DemandScore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PricePerShare = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Valuation = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyAnalyticsSnapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserPortfolioSnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SnapshotAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalInvested = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PortfolioValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnrealizedPnL = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPortfolioSnapshots", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PropertyAnalyticsSnapshots");

            migrationBuilder.DropTable(
                name: "UserPortfolioSnapshots");
        }
    }
}
