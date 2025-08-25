using Microsoft.EntityFrameworkCore;
using HRMS.Backend.Models;

namespace HRMS.Backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // === DbSets ===
        public DbSet<Tenant> Tenants => Set<Tenant>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Organization> Organizations => Set<Organization>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Attendance> Attendances => Set<Attendance>();
        public DbSet<EmployeeRole> EmployeeRoles => Set<EmployeeRole>();
        public DbSet<Announcement> Announcements => Set<Announcement>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<Job> Jobs => Set<Job>();
        public DbSet<Applicant> Applicants => Set<Applicant>();
        public DbSet<Interview> Interviews => Set<Interview>();
        public DbSet<Leave> Leaves => Set<Leave>();
        public DbSet<LeaveType> LeaveTypes => Set<LeaveType>();
        public DbSet<Goal> Goals => Set<Goal>();
        public DbSet<PerformanceReview> PerformanceReviews => Set<PerformanceReview>();
        public DbSet<RequestFeedback> RequestFeedbacks => Set<RequestFeedback>();
        public DbSet<Training> Trainings => Set<Training>();
        public DbSet<TrainingEnrollment> TrainingEnrollments => Set<TrainingEnrollment>();
        public DbSet<Asset> Assets => Set<Asset>();
        public DbSet<TenantSetting> TenantSettings => Set<TenantSetting>();
        public DbSet<OrgSetting> OrgSettings => Set<OrgSetting>();

        protected override void OnModelCreating(ModelBuilder model)
        {
            base.OnModelCreating(model);
            var modelBuilder = model; // alias so existing calls compile

            /* ===== TENANTS (diagram-compliant) ===== */
            model.Entity<Tenant>(e =>
            {
                e.ToTable("tenants");
                e.HasKey(x => x.Id);

                e.Property(x => x.Name).HasColumnName("name").IsRequired().HasMaxLength(200);
                e.Property(x => x.Domain).HasColumnName("domain").HasMaxLength(255);
                e.Property(x => x.ContactEmail).HasColumnName("contact_email").HasMaxLength(255);
                e.Property(x => x.ContactPhone).HasColumnName("contact_phone").HasMaxLength(50);
                e.Property(x => x.Address).HasColumnName("address").HasMaxLength(500);

                e.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("datetime2(3)").HasDefaultValueSql("SYSUTCDATETIME()");
                e.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("datetime2(3)").HasDefaultValueSql("SYSUTCDATETIME()");

                e.HasIndex(x => x.Domain).IsUnique().HasFilter("[domain] IS NOT NULL");
            });

            /* ===== ORGANIZATIONS (diagram-compliant) ===== */
            model.Entity<Organization>(e =>
            {
                e.ToTable("organizations");
                e.HasKey(x => x.Id);

                e.Property(x => x.TenantId).HasColumnName("tenant_id");
                e.Property(x => x.Name).HasColumnName("organization_name").IsRequired().HasMaxLength(200);
                e.Property(x => x.OrgCode).HasColumnName("org_code").HasMaxLength(50);
                e.Property(x => x.Domain).HasColumnName("domain").HasMaxLength(255);
                e.Property(x => x.Industry).HasColumnName("industry").HasMaxLength(100);
                e.Property(x => x.Location).HasColumnName("location").HasMaxLength(200);
                e.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("datetime2(3)").HasDefaultValueSql("SYSUTCDATETIME()");
                e.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("datetime2(3)").HasDefaultValueSql("SYSUTCDATETIME()");

                e.HasOne(x => x.Tenant)
                 .WithMany(t => t.Organizations)
                 .HasForeignKey(x => x.TenantId)
                 .OnDelete(DeleteBehavior.Restrict);

                // Alternate key for tenant-safe refs
                e.HasAlternateKey(x => new { x.Id, x.TenantId }).HasName("AK_organizations_id_tenant");

                e.HasIndex(x => new { x.TenantId, x.OrgCode }).IsUnique().HasFilter("[org_code] IS NOT NULL");
                e.HasIndex(x => new { x.TenantId, x.Domain }).IsUnique().HasFilter("[domain] IS NOT NULL");
            });

            /* ===== DEPARTMENTS (diagram-compliant) ===== */
            model.Entity<Department>(e =>
            {
                e.ToTable("departments");
                e.HasKey(x => x.Id);

                e.Property(x => x.OrganizationId).HasColumnName("organization_id");
                e.Property(x => x.TenantId).HasColumnName("tenant_id");
                e.Property(x => x.DepartmentName).HasColumnName("name").IsRequired().HasMaxLength(200);
                e.Property(x => x.Description).HasColumnName("description");
                e.Property(x => x.DepartmentCode).HasColumnName("department_code").HasMaxLength(50);
                e.Property(x => x.ParentDepartmentId).HasColumnName("parent_department_id").IsRequired(false);

                e.HasOne(x => x.Tenant)
                 .WithMany(t => t.Departments)
                 .HasForeignKey(x => x.TenantId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Organization)
                 .WithMany(o => o.Departments)
                 .HasForeignKey(x => new { x.OrganizationId, x.TenantId })
                 .HasPrincipalKey(o => new { o.Id, o.TenantId })
                 .OnDelete(DeleteBehavior.Restrict);

                // Alternate key for self refs + employee refs
                e.HasAlternateKey(x => new { x.Id, x.OrganizationId, x.TenantId })
                 .HasName("AK_departments_id_org_tenant");

                e.HasOne(x => x.ParentDepartment)
                 .WithMany(p => p.ChildDepartments)
                 .HasForeignKey(x => new { x.ParentDepartmentId, x.OrganizationId, x.TenantId })
                 .HasPrincipalKey(p => new { p.Id, p.OrganizationId, p.TenantId })
                 .OnDelete(DeleteBehavior.NoAction); // <— changed from SetNull

                e.HasIndex(x => new { x.OrganizationId, x.DepartmentName }).IsUnique();
                e.HasIndex(x => new { x.OrganizationId, x.DepartmentCode })
                 .IsUnique()
                 .HasFilter("[department_code] IS NOT NULL");
            });

            /* ===== EMPLOYEES (diagram-compliant) ===== */
            model.Entity<Employee>(e =>
            {
                e.ToTable("employees");
                e.HasKey(x => x.EmployeeID);

                e.Property(x => x.EmployeeID).HasColumnName("id");
                e.Property(x => x.DepartmentId).HasColumnName("department_id");
                e.Property(x => x.OrganizationId).HasColumnName("organization_id");
                e.Property(x => x.TenantId).HasColumnName("tenant_id");
                e.Property(x => x.ManagerId).HasColumnName("manager_id");

                e.Property(x => x.FirstName).HasColumnName("first_name").HasMaxLength(100).IsRequired();
                e.Property(x => x.MiddleName).HasColumnName("middle_name").HasMaxLength(100);
                e.Property(x => x.LastName).HasColumnName("last_name").HasMaxLength(100).IsRequired();

                e.Property(x => x.Email).HasColumnName("email").HasMaxLength(255);
                e.Property(x => x.PhoneNumber).HasColumnName("phone_number").HasMaxLength(50);
                e.Property(x => x.EmergencyContactName).HasColumnName("emergency_contact_name").HasMaxLength(200);
                e.Property(x => x.EmergencyContactNumber).HasColumnName("emergency_contact_number").HasMaxLength(50);
                e.Property(x => x.Gender).HasColumnName("gender").HasMaxLength(50);
                e.Property(x => x.Nationality).HasColumnName("nationality").HasMaxLength(100);
                e.Property(x => x.MaritalStatus).HasColumnName("marital_status").HasMaxLength(50);
                e.Property(x => x.Address).HasColumnName("address").HasMaxLength(500);
                e.Property(x => x.DateOfBirth).HasColumnName("date_of_birth");

                e.Property(x => x.JobTitle).HasColumnName("job_title").HasMaxLength(150);
                e.Property(x => x.EmployeeCode).HasColumnName("employee_code").HasMaxLength(50);
                e.Property(x => x.EmployeeEducationStatus).HasColumnName("employee_education_status").HasMaxLength(100);
                e.Property(x => x.EmploymentType).HasColumnName("employee_type").HasMaxLength(50);
                e.Property(x => x.PhotoUrl).HasColumnName("photo_url").HasMaxLength(2083);
                e.Property(x => x.JoiningDate).HasColumnName("hire_date");

                e.Property(x => x.BankDetails).HasColumnName("bank_details");
                e.Property(x => x.CustomFields).HasColumnName("custom_fields");

                e.Property(x => x.CreatedAt).HasColumnName("created_at")
                    .HasColumnType("datetime2(3)").HasDefaultValueSql("SYSUTCDATETIME()");
                e.Property(x => x.UpdatedAt).HasColumnName("updated_at")
                    .HasColumnType("datetime2(3)").HasDefaultValueSql("SYSUTCDATETIME()");
                e.Property(x => x.TerminatedDate).HasColumnName("terminated_date").HasColumnType("datetime2(3)");

                // JSON validity check
                e.ToTable(t => t.HasCheckConstraint("CHK_emp_custom_fields_json",
                    "custom_fields IS NULL OR ISJSON(custom_fields) = 1"));

                // ===== Relationships =====

                // Tenant
                e.HasOne(x => x.Tenant)
                 .WithMany(t => t.Employees)
                 .HasForeignKey(x => x.TenantId)
                 .OnDelete(DeleteBehavior.Restrict);

                // Organization (composite FK -> composite PK)
                e.HasOne(x => x.Organization)
                 .WithMany(o => o.Employees)
                 .HasForeignKey(x => new { x.OrganizationId, x.TenantId })
                 .HasPrincipalKey(o => new { o.Id, o.TenantId })
                 .OnDelete(DeleteBehavior.Restrict);

                // Department (composite)
                e.HasOne(x => x.Department)
                 .WithMany(d => d.Employees)
                 .HasForeignKey(x => new { x.DepartmentId, x.OrganizationId, x.TenantId })
                 .HasPrincipalKey(d => new { d.Id, d.OrganizationId, d.TenantId })
                 .OnDelete(DeleteBehavior.Restrict);

                // Alternate key used by dependents (e.g., Attendance)
                e.HasAlternateKey(x => new { x.EmployeeID, x.TenantId })
                 .HasName("AK_employees_id_tenant");

                // Self-manager (same tenant)
                e.HasOne(x => x.Manager)
                 .WithMany(m => m.Reports)
                 .HasForeignKey(x => new { x.ManagerId, x.TenantId })
                 .HasPrincipalKey(m => new { m.EmployeeID, m.TenantId })
                 .OnDelete(DeleteBehavior.Restrict);

                // Attendance (composite FK) — prevents shadow TenantId1/2 columns
                e.HasMany(x => x.Attendances)
                 .WithOne(a => a.Employee)
                 .HasForeignKey(a => new { a.EmployeeId, a.TenantId })
                 .HasPrincipalKey(x => new { x.EmployeeID, x.TenantId })
                 .OnDelete(DeleteBehavior.Cascade);

                // ===== Indexes / uniques =====
                e.HasIndex(x => new { x.TenantId, x.Email })
                 .IsUnique()
                 .HasFilter("[email] IS NOT NULL");

                e.HasIndex(x => new { x.TenantId, x.EmployeeCode })
                 .IsUnique()
                 .HasFilter("[employee_code] IS NOT NULL");

                e.HasIndex(x => new { x.OrganizationId, x.TenantId });
                e.HasIndex(x => new { x.DepartmentId, x.OrganizationId, x.TenantId });
            });


            /* ===== Light mappings for other entities (defaults are fine) ===== */


            // Attendance
            modelBuilder.Entity<Attendance>(a =>
            {
                a.ToTable("attendance");
                a.HasKey(x => x.Id);

                a.Property(x => x.Id).HasColumnName("Id");
                a.Property(x => x.EmployeeId).HasColumnName("employee_id");
                a.Property(x => x.TenantId).HasColumnName("tenant_id");

                a.Property(x => x.ClockIn).HasColumnName("clock_in").HasColumnType("datetime2(3)");
                a.Property(x => x.ClockOut).HasColumnName("clock_out").HasColumnType("datetime2(3)");
                a.Property(x => x.AttendanceDate).HasColumnName("attendance_date").HasColumnType("date");
                a.Property(x => x.Status).HasColumnName("status").HasMaxLength(50);
                a.Property(x => x.Location).HasColumnName("location");
                a.Property(x => x.IpAddress).HasColumnName("ip_address");
                a.Property(x => x.ShiftName).HasColumnName("shift_name");
                a.Property(x => x.Source).HasColumnName("source");
                a.Property(x => x.ExceptionNote).HasColumnName("exception_note");

                // FK to Employee uses composite (employee_id, tenant_id) → (id, tenant_id)
                a.HasOne(x => x.Employee)
                 .WithMany(e => e.Attendances)
                 .HasForeignKey(x => new { x.EmployeeId, x.TenantId })
                 .HasPrincipalKey(e => new { e.EmployeeID, e.TenantId })
                 .OnDelete(DeleteBehavior.Cascade);

                // (Optional) direct FK to Tenant (kept if you reference Attendance.Tenant)
                a.HasOne(x => x.Tenant)
                 .WithMany()
                 .HasForeignKey(x => x.TenantId)
                 .OnDelete(DeleteBehavior.Restrict);

                a.HasIndex(x => new { x.EmployeeId, x.TenantId, x.AttendanceDate })
                 .HasDatabaseName("IX_attendance_employee_id_tenant_id_attendance_date");
                a.HasIndex(x => new { x.TenantId, x.AttendanceDate })
                 .HasDatabaseName("IX_attendance_tenant_id_attendance_date");
            });




            model.Entity<EmployeeRole>(e =>
            {
                e.ToTable("employee_roles");
                e.HasKey(er => er.Id);
                e.HasOne(er => er.Employee)
                 .WithMany()
                 .HasForeignKey(er => er.EmployeeId)
                 .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(er => er.Role)
                 .WithMany()
                 .HasForeignKey(er => er.RoleId)
                 .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(er => er.Department)
                 .WithMany()
                 .HasForeignKey(er => er.DepartmentId)
                 .OnDelete(DeleteBehavior.Restrict);
                e.HasIndex(er => new { er.EmployeeId, er.DepartmentId, er.RoleId }).IsUnique();
            });
            model.Entity<Applicant>(e =>
            {
                e.ToTable("applicants");
                e.HasKey(a => a.Id);

                e.Property(a => a.Name).IsRequired().HasMaxLength(200);
                e.Property(a => a.Email).HasMaxLength(255);
                e.Property(a => a.Phone).HasMaxLength(50);
                e.Property(a => a.ResumeUrl).HasMaxLength(2083);
                e.Property(a => a.Status).HasMaxLength(50);
                e.Property(a => a.Source).HasMaxLength(100);
                e.Property(a => a.Notes); // nvarchar(max)

                // FK to jobs(id)
                e.HasOne(a => a.Job)
                 .WithMany(j => j.Applicants)                    // don’t require Job.Applicants property
                 .HasForeignKey(a => a.JobId)
                 .OnDelete(DeleteBehavior.Cascade);
            });
            // ===== LEAVES (resolve Employee vs Approver ambiguity) =====
            model.Entity<Leave>(e =>
            {
                e.ToTable("leaves");
                e.HasKey(l => l.Id);

                e.Property(l => l.EmployeeId).HasColumnName("employee_id");
                e.Property(l => l.TenantId).HasColumnName("tenant_id");
                e.Property(l => l.LeaveTypeId).HasColumnName("leave_type_id");
                e.Property(l => l.StartDate).HasColumnName("start_date").HasColumnType("date");
                e.Property(l => l.EndDate).HasColumnName("end_date").HasColumnType("date");
                e.Property(l => l.Status).HasColumnName("status").HasMaxLength(50);
                e.Property(l => l.ApprovedBy).HasColumnName("approved_by"); // nullable
                e.Property(l => l.Reason).HasColumnName("reason"); // nvarchar(max)
                e.Property(l => l.AppliedOn).HasColumnName("applied_on").HasColumnType("datetime2(3)");
                e.Property(l => l.ManagerComment).HasColumnName("manager_comment"); // nvarchar(max)

                // Employee (requester) — tenant-safe composite FK -> Employee AK (EmployeeID,TenantId)
                e.HasOne(l => l.Employee)
                 .WithMany()
                 .HasForeignKey(l => new { l.EmployeeId, l.TenantId })
                 .HasPrincipalKey(emp => new { emp.EmployeeID, emp.TenantId })
                 .OnDelete(DeleteBehavior.Restrict);

                // Approver (manager) — FK -> Employee PK (EmployeeID); ApprovedBy is nullable
                e.HasOne(l => l.Approver)
                 .WithMany()
                 .HasForeignKey(l => l.ApprovedBy)
                 .HasPrincipalKey(emp => emp.EmployeeID)
                 .OnDelete(DeleteBehavior.Restrict);

                // LeaveType
                e.HasOne(l => l.LeaveType)
                 .WithMany(lt => lt.Leaves)
                 .HasForeignKey(l => l.LeaveTypeId)
                 .OnDelete(DeleteBehavior.Restrict);

                // (Optional) Tenant FK for clarity (no nav collection needed on Tenant)
                e.HasOne(l => l.Tenant)
                 .WithMany()
                 .HasForeignKey(l => l.TenantId)
                 .OnDelete(DeleteBehavior.Restrict);

                // Helpful index for lookups
                e.HasIndex(l => new { l.TenantId, l.EmployeeId, l.StartDate, l.EndDate });
            });
            // ===== PERFORMANCE REVIEWS =====
            model.Entity<PerformanceReview>(e =>
            {
                e.ToTable("performance_reviews");
                e.HasKey(pr => pr.Id);

                e.Property(pr => pr.EmployeeId).HasColumnName("EmployeeId");
                e.Property(pr => pr.ReviewerId).HasColumnName("ReviewerId");
                e.Property(pr => pr.TechnicalSkill).HasColumnName("TechnicalSkill");
                e.Property(pr => pr.Communication).HasColumnName("Communication");
                e.Property(pr => pr.Leadership).HasColumnName("Leadership");
                e.Property(pr => pr.Innovation).HasColumnName("Innovation");
                e.Property(pr => pr.Teamwork).HasColumnName("Teamwork");
                e.Property(pr => pr.OverallFeedback).HasColumnName("OverallFeedback");
                e.Property(pr => pr.ReviewCycle).HasColumnName("ReviewCycle");
                e.Property(pr => pr.ReviewPeriodStart).HasColumnName("ReviewPeriodStart");
                e.Property(pr => pr.ReviewPeriodEnd).HasColumnName("ReviewPeriodEnd");

                // IMPORTANT: avoid multiple cascade paths
                e.HasOne(pr => pr.Employee)
                 .WithMany()
                 .HasForeignKey(pr => pr.EmployeeId)
                 .OnDelete(DeleteBehavior.NoAction);   // or Restrict

                e.HasOne(pr => pr.Reviewer)
                 .WithMany()
                 .HasForeignKey(pr => pr.ReviewerId)
                 .OnDelete(DeleteBehavior.NoAction);   // or Restrict
            });



        }
    }
}
