using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.Backend.Migrations
{
    /// <inheritdoc />
    public partial class TrainingModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrainingEnrollments_Trainings_TrainingId",
                table: "TrainingEnrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_TrainingEnrollments_employees_EmployeeId",
                table: "TrainingEnrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Trainings_organizations_OrganizationId",
                table: "Trainings");

            migrationBuilder.DropForeignKey(
                name: "FK_Trainings_tenants_TenantId",
                table: "Trainings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Trainings",
                table: "Trainings");

            migrationBuilder.DropIndex(
                name: "IX_Trainings_TenantId",
                table: "Trainings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TrainingEnrollments",
                table: "TrainingEnrollments");

            migrationBuilder.DropIndex(
                name: "IX_TrainingEnrollments_EmployeeId",
                table: "TrainingEnrollments");

            migrationBuilder.DropIndex(
                name: "IX_TrainingEnrollments_TrainingId",
                table: "TrainingEnrollments");

            migrationBuilder.DropColumn(
                name: "ContractFile",
                table: "Trainings");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Trainings");

            migrationBuilder.DropColumn(
                name: "Instructor",
                table: "Trainings");

            migrationBuilder.DropColumn(
                name: "MaxParticipant",
                table: "Trainings");

            migrationBuilder.DropColumn(
                name: "Mode",
                table: "Trainings");

            migrationBuilder.DropColumn(
                name: "VideoLink",
                table: "Trainings");

            migrationBuilder.DropColumn(
                name: "AttendanceStatus",
                table: "TrainingEnrollments");

            migrationBuilder.DropColumn(
                name: "Feedback",
                table: "TrainingEnrollments");

            migrationBuilder.RenameTable(
                name: "Trainings",
                newName: "training_programs");

            migrationBuilder.RenameTable(
                name: "TrainingEnrollments",
                newName: "training_enrollments");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "training_programs",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "Level",
                table: "training_programs",
                newName: "level");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "training_programs",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Category",
                table: "training_programs",
                newName: "category");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "training_programs",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "TenantId",
                table: "training_programs",
                newName: "tenant_id");

            migrationBuilder.RenameColumn(
                name: "OrganizationId",
                table: "training_programs",
                newName: "organization_id");

            migrationBuilder.RenameColumn(
                name: "MaxEnrollment",
                table: "training_programs",
                newName: "max_enrollment");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "training_programs",
                newName: "start_date_utc");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "training_programs",
                newName: "end_date_utc");

            migrationBuilder.RenameIndex(
                name: "IX_Trainings_OrganizationId",
                table: "training_programs",
                newName: "IX_training_programs_organization_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "training_enrollments",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "training_enrollments",
                newName: "employee_id");

            migrationBuilder.RenameColumn(
                name: "TrainingId",
                table: "training_enrollments",
                newName: "tenant_id");

            migrationBuilder.RenameColumn(
                name: "EnrolledOn",
                table: "training_enrollments",
                newName: "completed_on_utc");

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "training_programs",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "level",
                table: "training_programs",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "training_programs",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "category",
                table: "training_programs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "duration_hours",
                table: "training_programs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "instructor_name",
                table: "training_programs",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "enrolled_on_utc",
                table: "training_enrollments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "program_id",
                table: "training_enrollments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "progress_percent",
                table: "training_enrollments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_training_programs",
                table: "training_programs",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_training_enrollments",
                table: "training_enrollments",
                column: "id");

            migrationBuilder.CreateTable(
                name: "training_feedback",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    program_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    employee_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    rating = table.Column<int>(type: "int", nullable: false),
                    comment = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    submitted_on_utc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_training_feedback", x => x.id);
                    table.ForeignKey(
                        name: "FK_training_feedback_employees_employee_id_tenant_id",
                        columns: x => new { x.employee_id, x.tenant_id },
                        principalTable: "employees",
                        principalColumns: new[] { "id", "tenant_id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_training_feedback_training_programs_program_id",
                        column: x => x.program_id,
                        principalTable: "training_programs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "training_materials",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    program_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    url = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    file_path = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    uploaded_at_utc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_training_materials", x => x.id);
                    table.ForeignKey(
                        name: "FK_training_materials_training_programs_program_id",
                        column: x => x.program_id,
                        principalTable: "training_programs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "training_sessions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    program_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    starts_at_utc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ends_at_utc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    is_online = table.Column<bool>(type: "bit", nullable: false),
                    meeting_link = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_training_sessions", x => x.id);
                    table.ForeignKey(
                        name: "FK_training_sessions_training_programs_program_id",
                        column: x => x.program_id,
                        principalTable: "training_programs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_training_programs_tenant_id_organization_id_title",
                table: "training_programs",
                columns: new[] { "tenant_id", "organization_id", "title" });

            migrationBuilder.CreateIndex(
                name: "IX_training_enrollments_employee_id_tenant_id",
                table: "training_enrollments",
                columns: new[] { "employee_id", "tenant_id" });

            migrationBuilder.CreateIndex(
                name: "IX_training_enrollments_program_id_employee_id_tenant_id",
                table: "training_enrollments",
                columns: new[] { "program_id", "employee_id", "tenant_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_training_feedback_employee_id_tenant_id",
                table: "training_feedback",
                columns: new[] { "employee_id", "tenant_id" });

            migrationBuilder.CreateIndex(
                name: "IX_training_feedback_program_id_employee_id_tenant_id",
                table: "training_feedback",
                columns: new[] { "program_id", "employee_id", "tenant_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_training_materials_program_id",
                table: "training_materials",
                column: "program_id");

            migrationBuilder.CreateIndex(
                name: "IX_training_sessions_program_id_starts_at_utc",
                table: "training_sessions",
                columns: new[] { "program_id", "starts_at_utc" });

            migrationBuilder.AddForeignKey(
                name: "FK_training_enrollments_employees_employee_id_tenant_id",
                table: "training_enrollments",
                columns: new[] { "employee_id", "tenant_id" },
                principalTable: "employees",
                principalColumns: new[] { "id", "tenant_id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_training_enrollments_training_programs_program_id",
                table: "training_enrollments",
                column: "program_id",
                principalTable: "training_programs",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_training_programs_organizations_organization_id",
                table: "training_programs",
                column: "organization_id",
                principalTable: "organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_training_enrollments_employees_employee_id_tenant_id",
                table: "training_enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_training_enrollments_training_programs_program_id",
                table: "training_enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_training_programs_organizations_organization_id",
                table: "training_programs");

            migrationBuilder.DropTable(
                name: "training_feedback");

            migrationBuilder.DropTable(
                name: "training_materials");

            migrationBuilder.DropTable(
                name: "training_sessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_training_programs",
                table: "training_programs");

            migrationBuilder.DropIndex(
                name: "IX_training_programs_tenant_id_organization_id_title",
                table: "training_programs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_training_enrollments",
                table: "training_enrollments");

            migrationBuilder.DropIndex(
                name: "IX_training_enrollments_employee_id_tenant_id",
                table: "training_enrollments");

            migrationBuilder.DropIndex(
                name: "IX_training_enrollments_program_id_employee_id_tenant_id",
                table: "training_enrollments");

            migrationBuilder.DropColumn(
                name: "duration_hours",
                table: "training_programs");

            migrationBuilder.DropColumn(
                name: "instructor_name",
                table: "training_programs");

            migrationBuilder.DropColumn(
                name: "enrolled_on_utc",
                table: "training_enrollments");

            migrationBuilder.DropColumn(
                name: "program_id",
                table: "training_enrollments");

            migrationBuilder.DropColumn(
                name: "progress_percent",
                table: "training_enrollments");

            migrationBuilder.RenameTable(
                name: "training_programs",
                newName: "Trainings");

            migrationBuilder.RenameTable(
                name: "training_enrollments",
                newName: "TrainingEnrollments");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "Trainings",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "level",
                table: "Trainings",
                newName: "Level");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "Trainings",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "category",
                table: "Trainings",
                newName: "Category");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Trainings",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "tenant_id",
                table: "Trainings",
                newName: "TenantId");

            migrationBuilder.RenameColumn(
                name: "organization_id",
                table: "Trainings",
                newName: "OrganizationId");

            migrationBuilder.RenameColumn(
                name: "max_enrollment",
                table: "Trainings",
                newName: "MaxEnrollment");

            migrationBuilder.RenameColumn(
                name: "start_date_utc",
                table: "Trainings",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "end_date_utc",
                table: "Trainings",
                newName: "EndDate");

            migrationBuilder.RenameIndex(
                name: "IX_training_programs_organization_id",
                table: "Trainings",
                newName: "IX_Trainings_OrganizationId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "TrainingEnrollments",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "employee_id",
                table: "TrainingEnrollments",
                newName: "EmployeeId");

            migrationBuilder.RenameColumn(
                name: "tenant_id",
                table: "TrainingEnrollments",
                newName: "TrainingId");

            migrationBuilder.RenameColumn(
                name: "completed_on_utc",
                table: "TrainingEnrollments",
                newName: "EnrolledOn");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Trainings",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Level",
                table: "Trainings",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Trainings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "Trainings",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "ContractFile",
                table: "Trainings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Duration",
                table: "Trainings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Instructor",
                table: "Trainings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MaxParticipant",
                table: "Trainings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Mode",
                table: "Trainings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VideoLink",
                table: "Trainings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttendanceStatus",
                table: "TrainingEnrollments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Feedback",
                table: "TrainingEnrollments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Trainings",
                table: "Trainings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TrainingEnrollments",
                table: "TrainingEnrollments",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Trainings_TenantId",
                table: "Trainings",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingEnrollments_EmployeeId",
                table: "TrainingEnrollments",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingEnrollments_TrainingId",
                table: "TrainingEnrollments",
                column: "TrainingId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrainingEnrollments_Trainings_TrainingId",
                table: "TrainingEnrollments",
                column: "TrainingId",
                principalTable: "Trainings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TrainingEnrollments_employees_EmployeeId",
                table: "TrainingEnrollments",
                column: "EmployeeId",
                principalTable: "employees",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Trainings_organizations_OrganizationId",
                table: "Trainings",
                column: "OrganizationId",
                principalTable: "organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Trainings_tenants_TenantId",
                table: "Trainings",
                column: "TenantId",
                principalTable: "tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
