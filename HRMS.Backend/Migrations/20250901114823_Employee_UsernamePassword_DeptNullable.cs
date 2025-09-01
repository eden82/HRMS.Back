using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.Backend.Migrations
{
    /// <inheritdoc />
    public partial class Employee_UsernamePassword_DeptNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_employees_tenant_id_employee_code",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "bank_account_number",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "certification_path",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "contract_file_path",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "currency",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "passport_number",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "payment_method",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "resume_path",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "tax_identification_number",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "work_location",
                table: "employees");

            migrationBuilder.RenameColumn(
                name: "salary",
                table: "employees",
                newName: "password_hash");

            migrationBuilder.AlterColumn<string>(
                name: "username",
                table: "employees",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "shift_details",
                table: "employees",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "employee_code",
                table: "employees",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<Guid>(
                name: "department_id",
                table: "employees",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "benefits_enrollment",
                table: "employees",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_employees_tenant_id_employee_code",
                table: "employees",
                columns: new[] { "tenant_id", "employee_code" },
                unique: true,
                filter: "[employee_code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_employees_tenant_id_username",
                table: "employees",
                columns: new[] { "tenant_id", "username" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_employees_tenant_id_employee_code",
                table: "employees");

            migrationBuilder.DropIndex(
                name: "IX_employees_tenant_id_username",
                table: "employees");

            migrationBuilder.RenameColumn(
                name: "password_hash",
                table: "employees",
                newName: "salary");

            migrationBuilder.AlterColumn<string>(
                name: "username",
                table: "employees",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "shift_details",
                table: "employees",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "employee_code",
                table: "employees",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "department_id",
                table: "employees",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "benefits_enrollment",
                table: "employees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "bank_account_number",
                table: "employees",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "certification_path",
                table: "employees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "contract_file_path",
                table: "employees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "currency",
                table: "employees",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "passport_number",
                table: "employees",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "payment_method",
                table: "employees",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "resume_path",
                table: "employees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "tax_identification_number",
                table: "employees",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "work_location",
                table: "employees",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_employees_tenant_id_employee_code",
                table: "employees",
                columns: new[] { "tenant_id", "employee_code" },
                unique: true);
        }
    }
}
