using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sadkah.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedUnassignedRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "ROLE_UNASSIGNED", "ROLE_UNASSIGNED", "Unassigned", "UNASSIGNED" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ROLE_UNASSIGNED");
        }
    }
}
