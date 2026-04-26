using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rentify.DataAccess.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class Add_Subscription_to_TenantEmergencyContact_And_Remove_Nav_For_MediaFileLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubscriptionId",
                table: "TenantEmergencyContacts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TenantEmergencyContacts_SubscriptionId",
                table: "TenantEmergencyContacts",
                column: "SubscriptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantEmergencyContacts_Subscriptions_SubscriptionId",
                table: "TenantEmergencyContacts",
                column: "SubscriptionId",
                principalTable: "Subscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenantEmergencyContacts_Subscriptions_SubscriptionId",
                table: "TenantEmergencyContacts");

            migrationBuilder.DropIndex(
                name: "IX_TenantEmergencyContacts_SubscriptionId",
                table: "TenantEmergencyContacts");

            migrationBuilder.DropColumn(
                name: "SubscriptionId",
                table: "TenantEmergencyContacts");
        }
    }
}
