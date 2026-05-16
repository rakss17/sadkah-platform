using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sadkah.API.Migrations
{
    /// <inheritdoc />
    public partial class AddRestrictDeleteBehaviorToCampaignAndDonation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Campaigns_AspNetUsers_OwnerId",
                table: "Campaigns");

            migrationBuilder.DropForeignKey(
                name: "FK_Donations_AspNetUsers_DonorId",
                table: "Donations");

            migrationBuilder.AlterColumn<string>(
                name: "DonorId",
                table: "Donations",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Campaigns_AspNetUsers_OwnerId",
                table: "Campaigns",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Donations_AspNetUsers_DonorId",
                table: "Donations",
                column: "DonorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Campaigns_AspNetUsers_OwnerId",
                table: "Campaigns");

            migrationBuilder.DropForeignKey(
                name: "FK_Donations_AspNetUsers_DonorId",
                table: "Donations");

            migrationBuilder.AlterColumn<string>(
                name: "DonorId",
                table: "Donations",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Campaigns_AspNetUsers_OwnerId",
                table: "Campaigns",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Donations_AspNetUsers_DonorId",
                table: "Donations",
                column: "DonorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
