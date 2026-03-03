using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateInvesting.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyDocumentTitleAndFileName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DocumentName",
                table: "PropertyDocuments",
                newName: "Title");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "PropertyDocuments",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "PropertyDocuments");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "PropertyDocuments",
                newName: "DocumentName");
        }
    }
}
