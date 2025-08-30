using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.Backend.Migrations
{
    /// <inheritdoc />
    public partial class EmployeeRequiredFields_CodesUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "middle_name",
                table: "employees",
                newName: "payment_method");

            migrationBuilder.AddColumn<string>(
                name: "bank_account_number",
                table: "employees",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "benefits_enrollment",
                table: "employees",
                type: "nvarchar(max)",
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
                name: "resume_path",
                table: "employees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "salary",
                table: "employees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "shift_details",
                table: "employees",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "tax_identification_number",
                table: "employees",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "username",
                table: "employees",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "work_location",
                table: "employees",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "bank_account_number",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "benefits_enrollment",
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
                name: "resume_path",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "salary",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "shift_details",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "tax_identification_number",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "username",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "work_location",
                table: "employees");

            migrationBuilder.RenameColumn(
                name: "payment_method",
                table: "employees",
                newName: "middle_name");
        }
    }
}
