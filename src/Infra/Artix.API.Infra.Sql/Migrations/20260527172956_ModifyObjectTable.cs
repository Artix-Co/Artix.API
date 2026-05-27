using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Artix.API.Infra.Sql.Migrations
{
    /// <inheritdoc />
    public partial class ModifyObjectTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GeneralInformation",
                table: "Objects");

            migrationBuilder.DropColumn(
                name: "SpecialInformation",
                table: "Objects");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Objects",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Objects");

            migrationBuilder.AddColumn<string>(
                name: "GeneralInformation",
                table: "Objects",
                type: "nvarchar(max)",
                maxLength: 10000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpecialInformation",
                table: "Objects",
                type: "nvarchar(max)",
                maxLength: 10000,
                nullable: true);
        }
    }
}
