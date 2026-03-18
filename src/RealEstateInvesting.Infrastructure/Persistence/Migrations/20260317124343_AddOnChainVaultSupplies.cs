using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateInvesting.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOnChainVaultSupplies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OnChainVaultSupplies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VaultAddress = table.Column<string>(type: "nvarchar(42)", maxLength: 42, nullable: false),
                    TokenAddress = table.Column<string>(type: "nvarchar(42)", maxLength: 42, nullable: false),
                    AmountMintedRaw = table.Column<string>(type: "nvarchar(78)", maxLength: 78, nullable: false),
                    IdentityTxHash = table.Column<string>(type: "nvarchar(66)", maxLength: 66, nullable: true),
                    MintTxHash = table.Column<string>(type: "nvarchar(66)", maxLength: 66, nullable: false),
                    PropertyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PerformedByAdminId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnChainVaultSupplies", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OnChainVaultSupplies_CreatedAt",
                table: "OnChainVaultSupplies",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OnChainVaultSupplies_PropertyId",
                table: "OnChainVaultSupplies",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_OnChainVaultSupplies_TokenAddress",
                table: "OnChainVaultSupplies",
                column: "TokenAddress");

            migrationBuilder.CreateIndex(
                name: "IX_OnChainVaultSupplies_VaultAddress",
                table: "OnChainVaultSupplies",
                column: "VaultAddress");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OnChainVaultSupplies");
        }
    }
}
