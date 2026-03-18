using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateInvesting.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOnChainShareSales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OnChainShareSales",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserWalletAddress = table.Column<string>(type: "nvarchar(42)", maxLength: 42, nullable: true),
                    PropertyTokenAddress = table.Column<string>(type: "nvarchar(42)", maxLength: 42, nullable: false),
                    AmountOfSharesRaw = table.Column<string>(type: "nvarchar(78)", maxLength: 78, nullable: false),
                    ApproveTxHash = table.Column<string>(type: "nvarchar(66)", maxLength: 66, nullable: true),
                    SellTxHash = table.Column<string>(type: "nvarchar(66)", maxLength: 66, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnChainShareSales", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OnChainShareSales_CreatedAt",
                table: "OnChainShareSales",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OnChainShareSales_PropertyTokenAddress",
                table: "OnChainShareSales",
                column: "PropertyTokenAddress");

            migrationBuilder.CreateIndex(
                name: "IX_OnChainShareSales_UserId",
                table: "OnChainShareSales",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OnChainShareSales");
        }
    }
}
