using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rentify.DataAccess.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class Change_on_delete_Media_file_link_tag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaFileLinkTags_MediaFileLinks_MediaFileLinkId",
                table: "MediaFileLinkTags");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaFileLinkTags_MediaFileLinks_MediaFileLinkId",
                table: "MediaFileLinkTags",
                column: "MediaFileLinkId",
                principalTable: "MediaFileLinks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaFileLinkTags_MediaFileLinks_MediaFileLinkId",
                table: "MediaFileLinkTags");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaFileLinkTags_MediaFileLinks_MediaFileLinkId",
                table: "MediaFileLinkTags",
                column: "MediaFileLinkId",
                principalTable: "MediaFileLinks",
                principalColumn: "Id");
        }
    }
}
