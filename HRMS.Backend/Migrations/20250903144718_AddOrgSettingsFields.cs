using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddOrgSettingsFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_org_settings_organizations_organization_id_tenant_id",
                table: "org_settings");

            migrationBuilder.DropIndex(
                name: "IX_org_settings_organization_id_tenant_id",
                table: "org_settings");

            migrationBuilder.DropColumn(
                name: "min_hours_full_day",
                table: "org_settings");

            migrationBuilder.DropColumn(
                name: "min_hours_half_day",
                table: "org_settings");

            migrationBuilder.DropColumn(
                name: "time_zone_id",
                table: "org_settings");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "org_settings",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "shift_start",
                table: "org_settings",
                newName: "workday_start");

            migrationBuilder.RenameColumn(
                name: "shift_end",
                table: "org_settings",
                newName: "workday_end");

            migrationBuilder.RenameColumn(
                name: "late_cutoff_minutes",
                table: "org_settings",
                newName: "late_after_minutes");

            migrationBuilder.RenameColumn(
                name: "grace_minutes",
                table: "org_settings",
                newName: "halfday_under_hours");

            migrationBuilder.AddColumn<bool>(
                name: "absent_if_no_clockin",
                table: "org_settings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "org_settings",
                type: "datetime2(3)",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()");

            migrationBuilder.AddColumn<string>(
                name: "time_zone",
                table: "org_settings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "org_settings",
                type: "datetime2(3)",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()");

            migrationBuilder.CreateIndex(
                name: "IX_org_settings_organization_id",
                table: "org_settings",
                column: "organization_id");

            migrationBuilder.AddForeignKey(
                name: "FK_org_settings_organizations_organization_id",
                table: "org_settings",
                column: "organization_id",
                principalTable: "organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_org_settings_tenants_tenant_id",
                table: "org_settings",
                column: "tenant_id",
                principalTable: "tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_org_settings_organizations_organization_id",
                table: "org_settings");

            migrationBuilder.DropForeignKey(
                name: "FK_org_settings_tenants_tenant_id",
                table: "org_settings");

            migrationBuilder.DropIndex(
                name: "IX_org_settings_organization_id",
                table: "org_settings");

            migrationBuilder.DropColumn(
                name: "absent_if_no_clockin",
                table: "org_settings");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "org_settings");

            migrationBuilder.DropColumn(
                name: "time_zone",
                table: "org_settings");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "org_settings");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "org_settings",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "workday_start",
                table: "org_settings",
                newName: "shift_start");

            migrationBuilder.RenameColumn(
                name: "workday_end",
                table: "org_settings",
                newName: "shift_end");

            migrationBuilder.RenameColumn(
                name: "late_after_minutes",
                table: "org_settings",
                newName: "late_cutoff_minutes");

            migrationBuilder.RenameColumn(
                name: "halfday_under_hours",
                table: "org_settings",
                newName: "grace_minutes");

            migrationBuilder.AddColumn<double>(
                name: "min_hours_full_day",
                table: "org_settings",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "min_hours_half_day",
                table: "org_settings",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "time_zone_id",
                table: "org_settings",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_org_settings_organization_id_tenant_id",
                table: "org_settings",
                columns: new[] { "organization_id", "tenant_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_org_settings_organizations_organization_id_tenant_id",
                table: "org_settings",
                columns: new[] { "organization_id", "tenant_id" },
                principalTable: "organizations",
                principalColumns: new[] { "Id", "tenant_id" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
