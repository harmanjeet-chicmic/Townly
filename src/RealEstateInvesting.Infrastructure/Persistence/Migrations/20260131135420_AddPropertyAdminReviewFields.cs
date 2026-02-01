using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateInvesting.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyAdminReviewFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "Properties",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewedAt",
                table: "Properties",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReviewedBy",
                table: "Properties",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "ReviewedAt",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "ReviewedBy",
                table: "Properties");
        }
    }
}
