using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Artix.API.Infra.Sql.Migrations
{
    /// <inheritdoc />
    public partial class AddObjectContentWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentStatus",
                table: "Objects",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "Draft");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentStatus",
                table: "Objects");
        }
    }
}
