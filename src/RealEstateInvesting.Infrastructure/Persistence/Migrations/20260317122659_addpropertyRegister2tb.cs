using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateInvesting.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addpropertyRegister2tb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OnChainPropertyRegistrations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ToAddress = table.Column<string>(type: "nvarchar(42)", maxLength: 42, nullable: false),
                    Uri = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TokenAddress = table.Column<string>(type: "nvarchar(42)", maxLength: 42, nullable: false),
                    VaultAddress = table.Column<string>(type: "nvarchar(42)", maxLength: 42, nullable: false),
                    OnChainPropertyId = table.Column<long>(type: "bigint", nullable: true),
                    TransactionHash = table.Column<string>(type: "nvarchar(66)", maxLength: 66, nullable: false),
                    PerformedByAdminId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnChainPropertyRegistrations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OnChainSharePurchases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserWalletAddress = table.Column<string>(type: "nvarchar(42)", maxLength: 42, nullable: true),
                    PropertyTokenAddress = table.Column<string>(type: "nvarchar(42)", maxLength: 42, nullable: false),
                    AmountOfSharesRaw = table.Column<string>(type: "nvarchar(78)", maxLength: 78, nullable: false),
                    AmountStablecoinApprovedRaw = table.Column<string>(type: "nvarchar(78)", maxLength: 78, nullable: false),
                    ApproveTxHash = table.Column<string>(type: "nvarchar(66)", maxLength: 66, nullable: true),
                    BuyTxHash = table.Column<string>(type: "nvarchar(66)", maxLength: 66, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnChainSharePurchases", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OnChainPropertyRegistrations_CreatedAt",
                table: "OnChainPropertyRegistrations",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OnChainPropertyRegistrations_PropertyId",
                table: "OnChainPropertyRegistrations",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_OnChainPropertyRegistrations_TransactionHash",
                table: "OnChainPropertyRegistrations",
                column: "TransactionHash");

            migrationBuilder.CreateIndex(
                name: "IX_OnChainSharePurchases_CreatedAt",
                table: "OnChainSharePurchases",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OnChainSharePurchases_PropertyTokenAddress",
                table: "OnChainSharePurchases",
                column: "PropertyTokenAddress");

            migrationBuilder.CreateIndex(
                name: "IX_OnChainSharePurchases_UserId",
                table: "OnChainSharePurchases",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OnChainPropertyRegistrations");

            migrationBuilder.DropTable(
                name: "OnChainSharePurchases");
        }
    }
}
