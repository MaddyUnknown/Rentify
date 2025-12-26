using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rentify.DataAccess.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class Remove_FailedCount_and_Retrying_status_from_MediaFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MediaFileLinks_MediaFileId",
                table: "MediaFileLinks");

            migrationBuilder.DropColumn(
                name: "FailureCount",
                table: "MediaFiles");

            migrationBuilder.CreateIndex(
                name: "IX_MediaFileLinks_MediaFileId",
                table: "MediaFileLinks",
                column: "MediaFileId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MediaFileLinks_MediaFileId",
                table: "MediaFileLinks");

            migrationBuilder.AddColumn<int>(
                name: "FailureCount",
                table: "MediaFiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_MediaFileLinks_MediaFileId",
                table: "MediaFileLinks",
                column: "MediaFileId");
        }
    }
}
