using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Artix.API.Infra.Sql.Migrations
{
    /// <inheritdoc />
    public partial class update_user_session : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserSession_ExpiresAt",
                table: "UserSessions");

            migrationBuilder.DropIndex(
                name: "IX_UserSession_RevokedAt",
                table: "UserSessions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_UserSession_ExpiresAfterCreated",
                table: "UserSessions");

            migrationBuilder.RenameColumn(
                name: "ExpiresAt",
                table: "UserSessions",
                newName: "RefreshTokenExpiresAt");

            migrationBuilder.RenameIndex(
                name: "UX_UserSession_JwtId",
                table: "UserSessions",
                newName: "IX_UserSessions_JwtId");

            migrationBuilder.RenameIndex(
                name: "IX_UserSession_UserId_RevokedAt",
                table: "UserSessions",
                newName: "IX_UserSessions_UserId_RevokedAt");

            migrationBuilder.RenameIndex(
                name: "IX_UserSession_UserId",
                table: "UserSessions",
                newName: "IX_UserSessions_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "UserAgent",
                table: "UserSessions",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "RefreshTokenHash",
                table: "UserSessions",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "JwtId",
                table: "UserSessions",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "DeviceId",
                table: "UserSessions",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Fingerprint",
                table: "UserSessions",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastRefreshAt",
                table: "UserSessions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSeenAt",
                table: "UserSessions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "RefreshCount",
                table: "UserSessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "RefreshTokenFamilyId",
                table: "UserSessions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ReplacedBySessionId",
                table: "UserSessions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevokeReason",
                table: "UserSessions",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SessionId",
                table: "UserSessions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_DeviceId",
                table: "UserSessions",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_RefreshTokenFamilyId",
                table: "UserSessions",
                column: "RefreshTokenFamilyId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_RefreshTokenHash",
                table: "UserSessions",
                column: "RefreshTokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_UserId_LastSeenAt",
                table: "UserSessions",
                columns: new[] { "UserId", "LastSeenAt" });

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_UserId_RefreshTokenExpiresAt",
                table: "UserSessions",
                columns: new[] { "UserId", "RefreshTokenExpiresAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserSessions_DeviceId",
                table: "UserSessions");

            migrationBuilder.DropIndex(
                name: "IX_UserSessions_RefreshTokenFamilyId",
                table: "UserSessions");

            migrationBuilder.DropIndex(
                name: "IX_UserSessions_RefreshTokenHash",
                table: "UserSessions");

            migrationBuilder.DropIndex(
                name: "IX_UserSessions_UserId_LastSeenAt",
                table: "UserSessions");

            migrationBuilder.DropIndex(
                name: "IX_UserSessions_UserId_RefreshTokenExpiresAt",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "Fingerprint",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "LastRefreshAt",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "LastSeenAt",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "RefreshCount",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "RefreshTokenFamilyId",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "ReplacedBySessionId",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "RevokeReason",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "UserSessions");

            migrationBuilder.RenameColumn(
                name: "RefreshTokenExpiresAt",
                table: "UserSessions",
                newName: "ExpiresAt");

            migrationBuilder.RenameIndex(
                name: "IX_UserSessions_UserId_RevokedAt",
                table: "UserSessions",
                newName: "IX_UserSession_UserId_RevokedAt");

            migrationBuilder.RenameIndex(
                name: "IX_UserSessions_UserId",
                table: "UserSessions",
                newName: "IX_UserSession_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserSessions_JwtId",
                table: "UserSessions",
                newName: "UX_UserSession_JwtId");

            migrationBuilder.AlterColumn<string>(
                name: "UserAgent",
                table: "UserSessions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1024)",
                oldMaxLength: 1024);

            migrationBuilder.AlterColumn<string>(
                name: "RefreshTokenHash",
                table: "UserSessions",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "JwtId",
                table: "UserSessions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(64)",
                oldMaxLength: 64);

            migrationBuilder.CreateIndex(
                name: "IX_UserSession_ExpiresAt",
                table: "UserSessions",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserSession_RevokedAt",
                table: "UserSessions",
                column: "RevokedAt");

            migrationBuilder.AddCheckConstraint(
                name: "CK_UserSession_ExpiresAfterCreated",
                table: "UserSessions",
                sql: "[ExpiresAt] > [CreatedAt]");
        }
    }
}
