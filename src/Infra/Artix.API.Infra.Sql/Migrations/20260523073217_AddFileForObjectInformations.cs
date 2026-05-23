using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Artix.API.Infra.Sql.Migrations
{
    /// <inheritdoc />
    public partial class AddFileForObjectInformations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Objects_Name",
                table: "Objects");

            migrationBuilder.DropIndex(
                name: "IX_Objects_QrCode",
                table: "Objects");

            migrationBuilder.DropIndex(
                name: "IX_Museums_Name",
                table: "Museums");

            migrationBuilder.AlterColumn<string>(
                name: "SpecialInformation",
                table: "Objects",
                type: "nvarchar(max)",
                maxLength: 10000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "GeneralInformation",
                table: "Objects",
                type: "nvarchar(max)",
                maxLength: 10000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ObjectGeneralInformation",
                columns: table => new
                {
                    ObjectId = table.Column<long>(type: "bigint", nullable: false),
                    FileId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectGeneralInformation", x => new { x.FileId, x.ObjectId });
                    table.ForeignKey(
                        name: "FK_ObjectGeneralInformation_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ObjectGeneralInformation_Objects_ObjectId",
                        column: x => x.ObjectId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ObjectSpecialInformation",
                columns: table => new
                {
                    ObjectId = table.Column<long>(type: "bigint", nullable: false),
                    FileId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectSpecialInformation", x => new { x.FileId, x.ObjectId });
                    table.ForeignKey(
                        name: "FK_ObjectSpecialInformation_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ObjectSpecialInformation_Objects_ObjectId",
                        column: x => x.ObjectId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ObjectGeneralInformationFiles_FileId",
                table: "ObjectGeneralInformation",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectGeneralInformationFiles_ObjectId",
                table: "ObjectGeneralInformation",
                column: "ObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectSpecialInformationFiles_FileId",
                table: "ObjectSpecialInformation",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectSpecialInformationFiles_ObjectId",
                table: "ObjectSpecialInformation",
                column: "ObjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ObjectGeneralInformation");

            migrationBuilder.DropTable(
                name: "ObjectSpecialInformation");

            migrationBuilder.AlterColumn<string>(
                name: "SpecialInformation",
                table: "Objects",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 10000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "GeneralInformation",
                table: "Objects",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 10000,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Objects_Name",
                table: "Objects",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Objects_QrCode",
                table: "Objects",
                column: "QrCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Museums_Name",
                table: "Museums",
                column: "Name",
                unique: true);
        }
    }
}
