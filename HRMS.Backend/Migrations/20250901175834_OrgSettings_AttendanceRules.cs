using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.Backend.Migrations
{
    /// <inheritdoc />
    public partial class OrgSettings_AttendanceRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrgSettings_organizations_OrganizationId",
                table: "OrgSettings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrgSettings",
                table: "OrgSettings");

            migrationBuilder.DropIndex(
                name: "IX_OrgSettings_OrganizationId",
                table: "OrgSettings");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "OrgSettings");

            migrationBuilder.DropColumn(
                name: "Settings",
                table: "OrgSettings");

            migrationBuilder.RenameTable(
                name: "OrgSettings",
                newName: "org_settings");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "org_settings",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "TenantId",
                table: "org_settings",
                newName: "tenant_id");

            migrationBuilder.RenameColumn(
                name: "OrganizationId",
                table: "org_settings",
                newName: "organization_id");

            migrationBuilder.RenameColumn(
                name: "Version",
                table: "org_settings",
                newName: "late_cutoff_minutes");

            migrationBuilder.AddColumn<int>(
                name: "grace_minutes",
                table: "org_settings",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.AddColumn<TimeSpan>(
                name: "shift_end",
                table: "org_settings",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "shift_start",
                table: "org_settings",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<string>(
                name: "time_zone_id",
                table: "org_settings",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_org_settings",
                table: "org_settings",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_org_settings_organization_id_tenant_id",
                table: "org_settings",
                columns: new[] { "organization_id", "tenant_id" });

            migrationBuilder.CreateIndex(
                name: "IX_org_settings_tenant_id_organization_id",
                table: "org_settings",
                columns: new[] { "tenant_id", "organization_id" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_org_settings_organizations_organization_id_tenant_id",
                table: "org_settings",
                columns: new[] { "organization_id", "tenant_id" },
                principalTable: "organizations",
                principalColumns: new[] { "Id", "tenant_id" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_org_settings_organizations_organization_id_tenant_id",
                table: "org_settings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_org_settings",
                table: "org_settings");

            migrationBuilder.DropIndex(
                name: "IX_org_settings_organization_id_tenant_id",
                table: "org_settings");

            migrationBuilder.DropIndex(
                name: "IX_org_settings_tenant_id_organization_id",
                table: "org_settings");

            migrationBuilder.DropColumn(
                name: "grace_minutes",
                table: "org_settings");

            migrationBuilder.DropColumn(
                name: "min_hours_full_day",
                table: "org_settings");

            migrationBuilder.DropColumn(
                name: "min_hours_half_day",
                table: "org_settings");

            migrationBuilder.DropColumn(
                name: "shift_end",
                table: "org_settings");

            migrationBuilder.DropColumn(
                name: "shift_start",
                table: "org_settings");

            migrationBuilder.DropColumn(
                name: "time_zone_id",
                table: "org_settings");

            migrationBuilder.RenameTable(
                name: "org_settings",
                newName: "OrgSettings");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "OrgSettings",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "tenant_id",
                table: "OrgSettings",
                newName: "TenantId");

            migrationBuilder.RenameColumn(
                name: "organization_id",
                table: "OrgSettings",
                newName: "OrganizationId");

            migrationBuilder.RenameColumn(
                name: "late_cutoff_minutes",
                table: "OrgSettings",
                newName: "Version");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "OrgSettings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Settings",
                table: "OrgSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrgSettings",
                table: "OrgSettings",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrgSettings_OrganizationId",
                table: "OrgSettings",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrgSettings_organizations_OrganizationId",
                table: "OrgSettings",
                column: "OrganizationId",
                principalTable: "organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
