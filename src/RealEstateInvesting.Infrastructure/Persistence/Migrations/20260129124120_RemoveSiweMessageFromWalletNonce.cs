using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateInvesting.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSiweMessageFromWalletNonce : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SiweMessage",
                table: "WalletNonces");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SiweMessage",
                table: "WalletNonces",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
