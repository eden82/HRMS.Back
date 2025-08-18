using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.Backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrganizationModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "Organizations");

            migrationBuilder.RenameColumn(
                name: "Timezone",
                table: "Organizations",
                newName: "TimeZone");

            migrationBuilder.AddColumn<string>(
                name: "AdminEmail",
                table: "Organizations",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AdminFirstName",
                table: "Organizations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AdminLastName",
                table: "Organizations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AdminPhone",
                table: "Organizations",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AttendanceTimeTracking",
                table: "Organizations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CompanySize",
                table: "Organizations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CountryCode",
                table: "Organizations",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Organizations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Domain",
                table: "Organizations",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "EmployeeManagement",
                table: "Organizations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Industry",
                table: "Organizations",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "LeaveManagement",
                table: "Organizations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PerformanceManagement",
                table: "Organizations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RecruitmentATS",
                table: "Organizations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TrainingDevelopment",
                table: "Organizations",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminEmail",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "AdminFirstName",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "AdminLastName",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "AdminPhone",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "AttendanceTimeTracking",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "CompanySize",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "CountryCode",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "Domain",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "EmployeeManagement",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "Industry",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "LeaveManagement",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "PerformanceManagement",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "RecruitmentATS",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "TrainingDevelopment",
                table: "Organizations");

            migrationBuilder.RenameColumn(
                name: "TimeZone",
                table: "Organizations",
                newName: "Timezone");

            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "Organizations",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
