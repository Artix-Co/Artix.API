using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Artix.API.Infra.Sql.Migrations
{
    /// <inheritdoc />
    public partial class ObjectImageObjectSaleType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ObjectFiles");

            migrationBuilder.AddColumn<int>(
                name: "ObjectSaleType",
                table: "Objects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MuseumImages",
                columns: table => new
                {
                    MuseumId = table.Column<long>(type: "bigint", nullable: false),
                    FileId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MuseumImages", x => new { x.FileId, x.MuseumId });
                    table.ForeignKey(
                        name: "FK_MuseumImages_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MuseumImages_Museums_MuseumId",
                        column: x => x.MuseumId,
                        principalTable: "Museums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Object3DModels",
                columns: table => new
                {
                    ObjectId = table.Column<long>(type: "bigint", nullable: false),
                    FileId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Object3DModels", x => new { x.FileId, x.ObjectId });
                    table.ForeignKey(
                        name: "FK_Object3DModels_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Object3DModels_Objects_FileId",
                        column: x => x.FileId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjectImages",
                columns: table => new
                {
                    ObjectId = table.Column<long>(type: "bigint", nullable: false),
                    FileId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectImages", x => new { x.FileId, x.ObjectId });
                    table.ForeignKey(
                        name: "FK_ObjectImages_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ObjectImages_Objects_FileId",
                        column: x => x.FileId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ObjectFiles_FileId",
                table: "MuseumImages",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectFiles_ObjectId",
                table: "MuseumImages",
                column: "MuseumId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectFiles_FileId",
                table: "Object3DModels",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectFiles_ObjectId",
                table: "Object3DModels",
                column: "ObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectFiles_FileId",
                table: "ObjectImages",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectFiles_ObjectId",
                table: "ObjectImages",
                column: "ObjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MuseumImages");

            migrationBuilder.DropTable(
                name: "Object3DModels");

            migrationBuilder.DropTable(
                name: "ObjectImages");

            migrationBuilder.DropColumn(
                name: "ObjectSaleType",
                table: "Objects");

            migrationBuilder.CreateTable(
                name: "ObjectFiles",
                columns: table => new
                {
                    FileId = table.Column<long>(type: "bigint", nullable: false),
                    ObjectId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectFiles", x => new { x.FileId, x.ObjectId });
                    table.ForeignKey(
                        name: "FK_ObjectFiles_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ObjectFiles_Objects_FileId",
                        column: x => x.FileId,
                        principalTable: "Objects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ObjectFiles_FileId",
                table: "ObjectFiles",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectFiles_ObjectId",
                table: "ObjectFiles",
                column: "ObjectId");
        }
    }
}
