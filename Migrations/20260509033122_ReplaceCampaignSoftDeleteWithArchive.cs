using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sadkah.Backend.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceCampaignSoftDeleteWithArchive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "Campaigns",
                newName: "ArchivedAt");

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Campaigns",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Campaigns");

            migrationBuilder.RenameColumn(
                name: "ArchivedAt",
                table: "Campaigns",
                newName: "DeletedAt");
        }
    }
}
