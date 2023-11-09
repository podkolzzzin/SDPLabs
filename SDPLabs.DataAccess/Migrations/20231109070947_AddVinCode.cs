using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SDPLabs.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddVinCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VinCode",
                table: "Cars",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VinCode",
                table: "Cars");
        }
    }
}
