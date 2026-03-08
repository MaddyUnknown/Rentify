using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rentify.DataAccess.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class Move_CoverPic_Column_To_Property : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaFiles_Owners_OwnerId",
                table: "MediaFiles");

            migrationBuilder.DropColumn(
                name: "MarkAsCoverRequested",
                table: "MediaFileLinks");

            migrationBuilder.DropColumn(
                name: "MarkedAsCover",
                table: "MediaFileLinks");

            migrationBuilder.AddColumn<int>(
                name: "ActiveCoverPicId",
                table: "Properties",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequestedCoverPicId",
                table: "Properties",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Properties_ActiveCoverPicId",
                table: "Properties",
                column: "ActiveCoverPicId",
                unique: true,
                filter: "[ActiveCoverPicId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_RequestedCoverPicId",
                table: "Properties",
                column: "RequestedCoverPicId",
                unique: true,
                filter: "[RequestedCoverPicId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaFiles_Owners_OwnerId",
                table: "MediaFiles",
                column: "OwnerId",
                principalTable: "Owners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_MediaFiles_ActiveCoverPicId",
                table: "Properties",
                column: "ActiveCoverPicId",
                principalTable: "MediaFiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_MediaFiles_RequestedCoverPicId",
                table: "Properties",
                column: "RequestedCoverPicId",
                principalTable: "MediaFiles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaFiles_Owners_OwnerId",
                table: "MediaFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_Properties_MediaFiles_ActiveCoverPicId",
                table: "Properties");

            migrationBuilder.DropForeignKey(
                name: "FK_Properties_MediaFiles_RequestedCoverPicId",
                table: "Properties");

            migrationBuilder.DropIndex(
                name: "IX_Properties_ActiveCoverPicId",
                table: "Properties");

            migrationBuilder.DropIndex(
                name: "IX_Properties_RequestedCoverPicId",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "ActiveCoverPicId",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "RequestedCoverPicId",
                table: "Properties");

            migrationBuilder.AddColumn<bool>(
                name: "MarkAsCoverRequested",
                table: "MediaFileLinks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MarkedAsCover",
                table: "MediaFileLinks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_MediaFiles_Owners_OwnerId",
                table: "MediaFiles",
                column: "OwnerId",
                principalTable: "Owners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
