using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateInvesting.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIsHiddenFromOwnerToProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "PropertyUpdateRequests");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "PropertyUpdateRequests");

            migrationBuilder.DropColumn(
                name: "PropertyType",
                table: "PropertyUpdateRequests");

            migrationBuilder.AddColumn<bool>(
                name: "IsHiddenFromOwner",
                table: "Properties",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsHiddenFromOwner",
                table: "Properties");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "PropertyUpdateRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "PropertyUpdateRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PropertyType",
                table: "PropertyUpdateRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
