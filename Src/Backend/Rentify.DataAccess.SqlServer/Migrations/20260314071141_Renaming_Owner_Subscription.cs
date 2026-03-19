using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rentify.DataAccess.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class Renaming_Owner_Subscription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MediaFiles_Owners_OwnerId",
                table: "MediaFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_Properties_Owners_OwnerId",
                table: "Properties");

            migrationBuilder.DropForeignKey(
                name: "FK_Tenants_Owners_OwnerId",
                table: "Tenants");

            migrationBuilder.DropForeignKey(
                name: "FK_Units_Owners_OwnerId",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Owners");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Units",
                newName: "SubscriptionId");

            migrationBuilder.RenameIndex(
                name: "IX_Units_OwnerId",
                table: "Units",
                newName: "IX_Units_SubscriptionId");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Tenants",
                newName: "SubscriptionId");

            migrationBuilder.RenameIndex(
                name: "IX_Tenants_OwnerId",
                table: "Tenants",
                newName: "IX_Tenants_SubscriptionId");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Properties",
                newName: "SubscriptionId");

            migrationBuilder.RenameIndex(
                name: "IX_Properties_OwnerId",
                table: "Properties",
                newName: "IX_Properties_SubscriptionId");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "MediaFiles",
                newName: "SubscriptionId");

            migrationBuilder.RenameIndex(
                name: "IX_MediaFiles_OwnerId",
                table: "MediaFiles",
                newName: "IX_MediaFiles_SubscriptionId");

            migrationBuilder.AddColumn<int>(
                name: "OwnerUserId",
                table: "Owners",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ReferenceId",
                table: "Owners",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Owners_ReferenceId",
                table: "Owners",
                column: "ReferenceId",
                unique: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropIndex(
                name: "IX_Owners_ReferenceId",
                table: "Owners");

            migrationBuilder.DropColumn(
                name: "OwnerUserId",
                table: "Owners");

            migrationBuilder.DropColumn(
                name: "ReferenceId",
                table: "Owners");

            migrationBuilder.RenameColumn(
                name: "SubscriptionId",
                table: "Units",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Units_SubscriptionId",
                table: "Units",
                newName: "IX_Units_OwnerId");

            migrationBuilder.RenameColumn(
                name: "SubscriptionId",
                table: "Tenants",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Tenants_SubscriptionId",
                table: "Tenants",
                newName: "IX_Tenants_OwnerId");

            migrationBuilder.RenameColumn(
                name: "SubscriptionId",
                table: "Properties",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Properties_SubscriptionId",
                table: "Properties",
                newName: "IX_Properties_OwnerId");

            migrationBuilder.RenameColumn(
                name: "SubscriptionId",
                table: "MediaFiles",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_MediaFiles_SubscriptionId",
                table: "MediaFiles",
                newName: "IX_MediaFiles_OwnerId");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Owners",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_MediaFiles_Owners_OwnerId",
                table: "MediaFiles",
                column: "OwnerId",
                principalTable: "Owners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_Owners_OwnerId",
                table: "Properties",
                column: "OwnerId",
                principalTable: "Owners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tenants_Owners_OwnerId",
                table: "Tenants",
                column: "OwnerId",
                principalTable: "Owners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Units_Owners_OwnerId",
                table: "Units",
                column: "OwnerId",
                principalTable: "Owners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
