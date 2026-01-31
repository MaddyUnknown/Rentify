using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rentify.DataAccess.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class Add_Cover_Requested_Column : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "MarkAsCoverRequested",
                table: "MediaFileLinks",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MarkAsCoverRequested",
                table: "MediaFileLinks");
        }
    }
}
