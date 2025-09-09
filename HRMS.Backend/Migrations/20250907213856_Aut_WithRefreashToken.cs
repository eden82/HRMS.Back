using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.Backend.Migrations
{
    /// <inheritdoc />
    public partial class Aut_WithRefreashToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_tenants_TenantId",
                table: "users");

            migrationBuilder.DropColumn(
                name: "password",
                table: "users");

            migrationBuilder.RenameColumn(
                name: "TenantId",
                table: "users",
                newName: "tenant_id");

            migrationBuilder.RenameIndex(
                name: "IX_users_TenantId",
                table: "users",
                newName: "IX_users_tenant_id");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "users",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "access_failed_count",
                table: "users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "email_confirmed",
                table: "users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "employee_id",
                table: "users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_login_utc",
                table: "users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "lockout_end_utc",
                table: "users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "normalized_email",
                table: "users",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "normalized_username",
                table: "users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte[]>(
                name: "password_hash",
                table: "users",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "password_salt",
                table: "users",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<bool>(
                name: "phone_confirmed",
                table: "users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "row_version",
                table: "users",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "security_stamp",
                table: "users",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "tfa_secret",
                table: "users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "two_factor_enabled",
                table: "users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    JwtId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2(3)", nullable: false),
                    ExpiresAtUtc = table.Column<DateTime>(type: "datetime2(3)", nullable: false),
                    RevokedAtUtc = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    ReplacedByToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_tokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_refresh_tokens_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_users_employee_id",
                table: "users",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_normalized_email",
                table: "users",
                column: "normalized_email",
                unique: true,
                filter: "[normalized_email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_users_normalized_username",
                table: "users",
                column: "normalized_username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_username",
                table: "users",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_Token",
                table: "refresh_tokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_UserId",
                table: "refresh_tokens",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_users_employees_employee_id",
                table: "users",
                column: "employee_id",
                principalTable: "employees",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_users_tenants_tenant_id",
                table: "users",
                column: "tenant_id",
                principalTable: "tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_employees_employee_id",
                table: "users");

            migrationBuilder.DropForeignKey(
                name: "FK_users_tenants_tenant_id",
                table: "users");

            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropIndex(
                name: "IX_users_employee_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_normalized_email",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_normalized_username",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_username",
                table: "users");

            migrationBuilder.DropColumn(
                name: "access_failed_count",
                table: "users");

            migrationBuilder.DropColumn(
                name: "email_confirmed",
                table: "users");

            migrationBuilder.DropColumn(
                name: "employee_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "users");

            migrationBuilder.DropColumn(
                name: "last_login_utc",
                table: "users");

            migrationBuilder.DropColumn(
                name: "lockout_end_utc",
                table: "users");

            migrationBuilder.DropColumn(
                name: "normalized_email",
                table: "users");

            migrationBuilder.DropColumn(
                name: "normalized_username",
                table: "users");

            migrationBuilder.DropColumn(
                name: "password_hash",
                table: "users");

            migrationBuilder.DropColumn(
                name: "password_salt",
                table: "users");

            migrationBuilder.DropColumn(
                name: "phone_confirmed",
                table: "users");

            migrationBuilder.DropColumn(
                name: "row_version",
                table: "users");

            migrationBuilder.DropColumn(
                name: "security_stamp",
                table: "users");

            migrationBuilder.DropColumn(
                name: "tfa_secret",
                table: "users");

            migrationBuilder.DropColumn(
                name: "two_factor_enabled",
                table: "users");

            migrationBuilder.RenameColumn(
                name: "tenant_id",
                table: "users",
                newName: "TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_users_tenant_id",
                table: "users",
                newName: "IX_users_TenantId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "users",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "SYSUTCDATETIME()");

            migrationBuilder.AddColumn<string>(
                name: "password",
                table: "users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_users_tenants_TenantId",
                table: "users",
                column: "TenantId",
                principalTable: "tenants",
                principalColumn: "Id");
        }
    }
}
