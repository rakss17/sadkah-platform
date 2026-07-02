using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sadkah.API.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationToCampaign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AddressLine1",
                table: "Campaigns",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AddressLine2",
                table: "Campaigns",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Barangay",
                table: "Campaigns",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Campaigns",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Campaigns",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Province",
                table: "Campaigns",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressLine1",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "AddressLine2",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "Barangay",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Campaigns");

            migrationBuilder.DropColumn(
                name: "Province",
                table: "Campaigns");
        }
    }
}
