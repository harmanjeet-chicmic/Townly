using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateInvesting.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOnChainKycActions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OnChainKycActions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WalletAddress = table.Column<string>(type: "nvarchar(42)", maxLength: 42, nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ActionType = table.Column<int>(type: "int", nullable: false),
                    IdentityContractAddress = table.Column<string>(type: "nvarchar(42)", maxLength: 42, nullable: true),
                    CountryCode = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_OnChainKycActions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OnChainKycActions_CreatedAt",
                table: "OnChainKycActions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OnChainKycActions_UserId",
                table: "OnChainKycActions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OnChainKycActions_WalletAddress",
                table: "OnChainKycActions",
                column: "WalletAddress");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OnChainKycActions");
        }
    }
}
