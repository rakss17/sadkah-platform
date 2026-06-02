using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Sadkah.API.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyUserRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ROLE_CAMPAIGN_OWNER");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ROLE_DONOR");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ROLE_UNASSIGNED");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "ROLE_USER", "ROLE_USER", "User", "USER" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ROLE_USER");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "ROLE_CAMPAIGN_OWNER", "ROLE_CAMPAIGN_OWNER", "CampaignOwner", "CAMPAIGNOWNER" },
                    { "ROLE_DONOR", "ROLE_DONOR", "Donor", "DONOR" },
                    { "ROLE_UNASSIGNED", "ROLE_UNASSIGNED", "Unassigned", "UNASSIGNED" }
                });
        }
    }
}
