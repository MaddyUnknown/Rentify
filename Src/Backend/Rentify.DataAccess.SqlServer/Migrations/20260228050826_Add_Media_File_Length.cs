using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rentify.DataAccess.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class Add_Media_File_Length : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Length",
                table: "MediaFiles",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Length",
                table: "MediaFiles");
        }
    }
}
