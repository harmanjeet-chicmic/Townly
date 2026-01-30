using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateInvesting.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSiweMessageToWalletNonce : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Domain",
                table: "WalletNonces",
                newName: "SiweMessage");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SiweMessage",
                table: "WalletNonces",
                newName: "Domain");
        }
    }
}
