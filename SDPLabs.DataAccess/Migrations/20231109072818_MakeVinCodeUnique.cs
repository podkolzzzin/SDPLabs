using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDPLabs.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class MakeVinCodeUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Cars_VinCode",
                table: "Cars",
                column: "VinCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cars_VinCode",
                table: "Cars");
        }
    }
}
