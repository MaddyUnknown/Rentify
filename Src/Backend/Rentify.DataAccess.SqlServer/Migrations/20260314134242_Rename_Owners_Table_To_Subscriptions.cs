using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rentify.DataAccess.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class Rename_Owners_Table_To_Subscriptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaFiles_Owners_SubscriptionId",
                table: "MediaFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_Properties_Owners_SubscriptionId",
                table: "Properties");

            migrationBuilder.DropForeignKey(
                name: "FK_Tenants_Owners_SubscriptionId",
                table: "Tenants");

            migrationBuilder.DropForeignKey(
                name: "FK_Units_Owners_SubscriptionId",
                table: "Units");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Owners",
                table: "Owners");

            migrationBuilder.RenameTable(
                name: "Owners",
                newName: "Subscriptions");

            migrationBuilder.RenameIndex(
                name: "IX_Owners_ReferenceId",
                table: "Subscriptions",
                newName: "IX_Subscriptions_ReferenceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Subscriptions",
                table: "Subscriptions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaFiles_Subscriptions_SubscriptionId",
                table: "MediaFiles",
                column: "SubscriptionId",
                principalTable: "Subscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_Subscriptions_SubscriptionId",
                table: "Properties",
                column: "SubscriptionId",
                principalTable: "Subscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tenants_Subscriptions_SubscriptionId",
                table: "Tenants",
                column: "SubscriptionId",
                principalTable: "Subscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Units_Subscriptions_SubscriptionId",
                table: "Units",
                column: "SubscriptionId",
                principalTable: "Subscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaFiles_Subscriptions_SubscriptionId",
                table: "MediaFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_Properties_Subscriptions_SubscriptionId",
                table: "Properties");

            migrationBuilder.DropForeignKey(
                name: "FK_Tenants_Subscriptions_SubscriptionId",
                table: "Tenants");

            migrationBuilder.DropForeignKey(
                name: "FK_Units_Subscriptions_SubscriptionId",
                table: "Units");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Subscriptions",
                table: "Subscriptions");

            migrationBuilder.RenameTable(
                name: "Subscriptions",
                newName: "Owners");

            migrationBuilder.RenameIndex(
                name: "IX_Subscriptions_ReferenceId",
                table: "Owners",
                newName: "IX_Owners_ReferenceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Owners",
                table: "Owners",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaFiles_Owners_SubscriptionId",
                table: "MediaFiles",
                column: "SubscriptionId",
                principalTable: "Owners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_Owners_SubscriptionId",
                table: "Properties",
                column: "SubscriptionId",
                principalTable: "Owners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tenants_Owners_SubscriptionId",
                table: "Tenants",
                column: "SubscriptionId",
                principalTable: "Owners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Units_Owners_SubscriptionId",
                table: "Units",
                column: "SubscriptionId",
                principalTable: "Owners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
