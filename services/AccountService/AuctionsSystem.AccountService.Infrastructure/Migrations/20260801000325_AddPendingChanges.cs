using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuctionsSystem.AccountService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPendingChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_accounts_id_number",
                table: "accounts",
                column: "id_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_accounts_phone_number",
                table: "accounts",
                column: "phone_number",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_accounts_id_number",
                table: "accounts");

            migrationBuilder.DropIndex(
                name: "IX_accounts_phone_number",
                table: "accounts");
        }
    }
}
