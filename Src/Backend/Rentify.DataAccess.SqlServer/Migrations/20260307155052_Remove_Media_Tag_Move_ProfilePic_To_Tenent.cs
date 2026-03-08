using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rentify.DataAccess.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class Remove_Media_Tag_Move_ProfilePic_To_Tenent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MediaFileLinkTags");

            migrationBuilder.AddColumn<int>(
                name: "ProfilePicId",
                table: "Tenants",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_ProfilePicId",
                table: "Tenants",
                column: "ProfilePicId",
                unique: true,
                filter: "[ProfilePicId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Tenants_MediaFiles_ProfilePicId",
                table: "Tenants",
                column: "ProfilePicId",
                principalTable: "MediaFiles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tenants_MediaFiles_ProfilePicId",
                table: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_Tenants_ProfilePicId",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "ProfilePicId",
                table: "Tenants");

            migrationBuilder.CreateTable(
                name: "MediaFileLinkTags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MediaFileLinkId = table.Column<int>(type: "int", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Tag = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaFileLinkTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MediaFileLinkTags_MediaFileLinks_MediaFileLinkId",
                        column: x => x.MediaFileLinkId,
                        principalTable: "MediaFileLinks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MediaFileLinkTags_MediaFileLinkId",
                table: "MediaFileLinkTags",
                column: "MediaFileLinkId");
        }
    }
}
