using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Artix.API.Infra.Sql.Migrations;

public partial class AddFavorites : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Favorites",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                BusinessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UserId = table.Column<long>(type: "bigint", nullable: false),
                MuseumId = table.Column<long>(type: "bigint", nullable: true),
                ObjectId = table.Column<long>(type: "bigint", nullable: true),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "smalldatetime", nullable: false),
                ModifiedAt = table.Column<DateTime>(type: "smalldatetime", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Favorites", x => x.Id);
                table.ForeignKey("FK_Favorites_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Cascade);
                table.ForeignKey("FK_Favorites_Museums_MuseumId", x => x.MuseumId, "Museums", "Id", onDelete: ReferentialAction.Cascade);
                table.ForeignKey("FK_Favorites_Objects_ObjectId", x => x.ObjectId, "Objects", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.AddCheckConstraint("CK_Favorites_OneTarget", "Favorites", "([MuseumId] IS NOT NULL AND [ObjectId] IS NULL) OR ([MuseumId] IS NULL AND [ObjectId] IS NOT NULL)");
        migrationBuilder.CreateIndex("IX_Favorites_UserId_MuseumId_ObjectId", "Favorites", new[] { "UserId", "MuseumId", "ObjectId" }, unique: true);
        migrationBuilder.CreateIndex("IX_Favorites_UserId", "Favorites", "UserId");
    }

    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable("Favorites");
}

