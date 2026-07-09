using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sadkah.API.Migrations
{
    /// <inheritdoc />
    public partial class AddMethodAndReceiptToDonation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Method",
                table: "Donations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReceiptImagePublicId",
                table: "Donations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReceiptImageUrl",
                table: "Donations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Method",
                table: "Donations");

            migrationBuilder.DropColumn(
                name: "ReceiptImagePublicId",
                table: "Donations");

            migrationBuilder.DropColumn(
                name: "ReceiptImageUrl",
                table: "Donations");
        }
    }
}
