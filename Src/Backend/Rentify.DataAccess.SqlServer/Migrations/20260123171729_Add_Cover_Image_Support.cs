using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rentify.DataAccess.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class Add_Cover_Image_Support : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "MarkedAsCover",
                table: "MediaFileLinks",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MarkedAsCover",
                table: "MediaFileLinks");
        }
    }
}
