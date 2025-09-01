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

       

            /* ===== TENANTS ===== */
            model.Entity<Tenant>(e =>
            {
                e.ToTable("tenants");
                e.HasKey(x => x.Id);

                e.Property(x => x.Name).HasColumnName("tenant_name").IsRequired().HasMaxLength(200);
                e.Property(x => x.Domain).HasColumnName("domain").IsRequired().HasMaxLength(255); // REQUIRED now
                e.Property(x => x.Industry).HasColumnName("industry").HasMaxLength(100);
                e.Property(x => x.Location).HasColumnName("location").HasMaxLength(200);
                e.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("datetime2(3)").HasDefaultValueSql("SYSUTCDATETIME()");
                e.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("datetime2(3)").HasDefaultValueSql("SYSUTCDATETIME()");

                // Unique domain across tenants (no filter since NOT NULL)
                e.HasIndex(x => x.Domain).IsUnique();

                // (No IpRestrictions here anymore)
            });

            /* ===== ORGANIZATIONS ===== */
            model.Entity<Organization>(e =>
            {
                e.ToTable("organizations");
                e.HasKey(x => x.Id);

                // Allow Departments/Employees/Assets to reference (Id, TenantId)
                e.HasAlternateKey(x => new { x.Id, x.TenantId })
                 .HasName("AK_organizations_id_tenant");


                e.Property(x => x.TenantId).HasColumnName("tenant_id");
                e.Property(x => x.Name).HasColumnName("organization_name").IsRequired().HasMaxLength(200);
                e.Property(x => x.OrgCode).HasColumnName("org_code").HasMaxLength(50);
                e.Property(x => x.Domain).HasColumnName("domain").HasMaxLength(255).IsRequired();
                e.Property(x => x.Industry).HasColumnName("industry").IsRequired().HasMaxLength(100);
                e.Property(x => x.Location).HasColumnName("location").IsRequired().HasMaxLength(200);
                e.Property(x => x.LogoUrl).HasColumnName("logo_url").IsRequired().HasMaxLength(500);

                // NEW
                e.Property(x => x.IpRestrictions).HasColumnName("ip_restrictions").HasMaxLength(2048);

                e.Property(x => x.CreatedAt).HasColumnName("created_at").HasColumnType("datetime2(3)").HasDefaultValueSql("SYSUTCDATETIME()");
                e.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasColumnType("datetime2(3)").HasDefaultValueSql("SYSUTCDATETIME()");

                e.HasOne(x => x.Tenant)
                    .WithMany(t => t.Organizations)
                    .HasForeignKey(x => x.TenantId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(x => new { x.TenantId, x.OrgCode }).IsUnique().HasFilter("[org_code] IS NOT NULL");
                e.HasIndex(x => new { x.TenantId, x.Domain }).IsUnique().HasFilter("[domain] IS NOT NULL");
            });




            /* ===== DEPARTMENTS (GUID + composite) ===== */
            // In AppDbContext.OnModelCreating(ModelBuilder model)
            model.Entity<Department>(e =>
            {
                e.ToTable("departments");
                e.HasKey(x => x.Id);

                e.Property(x => x.OrganizationId).HasColumnName("organization_id");
                e.Property(x => x.TenantId).HasColumnName("tenant_id");
                e.Property(x => x.DepartmentName).HasColumnName("name").IsRequired().HasMaxLength(200);
                e.Property(x => x.Description).HasColumnName("description");
                e.Property(x => x.DepartmentCode).HasColumnName("department_code").HasMaxLength(50);
                e.Property(x => x.ParentDepartmentId).HasColumnName("parent_department_id");

                e.Property(x => x.DepartmentHeadId).HasColumnName("department_head_id");

                // Tenant
                e.HasOne(x => x.Tenant)
                 .WithMany(t => t.Departments)
                 .HasForeignKey(x => x.TenantId)
                 .OnDelete(DeleteBehavior.Restrict);

                // Organization (composite safety)
                e.HasOne(x => x.Organization)
                 .WithMany(o => o.Departments)
                 .HasForeignKey(x => new { x.OrganizationId, x.TenantId })
                 .HasPrincipalKey(o => new { o.Id, o.TenantId })
                 .OnDelete(DeleteBehavior.Restrict);

                // Alternate key to support composite refs (parent & employee FKs)
                e.HasAlternateKey(x => new { x.Id, x.OrganizationId, x.TenantId })
                 .HasName("AK_departments_id_org_tenant");

                // Parent department (within same org/tenant)
                e.HasOne(x => x.ParentDepartment)
                 .WithMany(p => p.ChildDepartments)
                 .HasForeignKey(x => new { x.ParentDepartmentId, x.OrganizationId, x.TenantId })
                 .HasPrincipalKey(p => new { p.Id, p.OrganizationId, p.TenantId })
                 .OnDelete(DeleteBehavior.NoAction);

                // Department head must be an employee in same tenant (and typically same org)
                e.HasOne(x => x.DepartmentHead)
                 .WithMany() // not adding back-collection
                 .HasForeignKey(x => new { x.DepartmentHeadId, x.TenantId })
                 .HasPrincipalKey(emp => new { emp.EmployeeID, emp.TenantId })
                 .OnDelete(DeleteBehavior.Restrict)
                 .IsRequired(false); 
                 
                // Uniques
                e.HasIndex(x => new { x.OrganizationId, x.DepartmentName }).IsUnique();
                e.HasIndex(x => new { x.OrganizationId, x.DepartmentCode })
                 .IsUnique()
                 .HasFilter("[department_code] IS NOT NULL");
            });


            /* ===== EMPLOYEES (GUID + composite) ===== */
            model.Entity<Employee>(e =>
            {
                e.ToTable("employees");
                e.HasKey(x => x.EmployeeID);

                // Keep composite AK so Attendance/Leave can reference (EmployeeID, TenantId)
                e.HasAlternateKey(x => new { x.EmployeeID, x.TenantId })
                 .HasName("AK_employees_id_tenant");

                // FKs & columns
                e.Property(x => x.EmployeeID)
                 .HasColumnName("id")
                 .HasDefaultValueSql("NEWSEQUENTIALID()");

                e.Property(x => x.DepartmentId).HasColumnName("department_id").IsRequired(false);   // ← now optional
                e.Property(x => x.OrganizationId).HasColumnName("organization_id").IsRequired();
                e.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
                e.Property(x => x.RoleId).HasColumnName("role_id").IsRequired();

                // Auth
                e.Property(x => x.Username).HasColumnName("username").HasMaxLength(100).IsRequired();
                e.Property(x => x.PasswordHash).HasColumnName("password_hash").IsRequired();

                // Required personal/contact fields
                e.Property(x => x.FirstName).HasColumnName("first_name").HasMaxLength(100).IsRequired();
                e.Property(x => x.LastName).HasColumnName("last_name").HasMaxLength(100).IsRequired();
                e.Property(x => x.Gender).HasColumnName("gender").HasMaxLength(50).IsRequired();
                e.Property(x => x.Nationality).HasColumnName("nationality").HasMaxLength(100).IsRequired();
                e.Property(x => x.MaritalStatus).HasColumnName("marital_status").HasMaxLength(50).IsRequired();
                e.Property(x => x.Address).HasColumnName("address").HasMaxLength(500).IsRequired();
                e.Property(x => x.DateOfBirth).HasColumnName("date_of_birth").IsRequired();
                e.Property(x => x.Email).HasColumnName("email").HasMaxLength(255).IsRequired();
                e.Property(x => x.PhoneNumber).HasColumnName("phone_number").HasMaxLength(50).IsRequired();
                e.Property(x => x.EmergencyContactName).HasColumnName("emergency_contact_name").HasMaxLength(200).IsRequired();
                e.Property(x => x.EmergencyContactNumber).HasColumnName("emergency_contact_number").HasMaxLength(50).IsRequired();

                // Job
                e.Property(x => x.JobTitle).HasColumnName("job_title").HasMaxLength(150).IsRequired();
                e.Property(x => x.EmployeeEducationStatus).HasColumnName("employee_education_status").HasMaxLength(100).IsRequired();
                e.Property(x => x.EmploymentType).HasColumnName("employee_type").HasMaxLength(50).IsRequired();
                e.Property(x => x.PhotoUrl).HasColumnName("photo_url").HasMaxLength(2083).IsRequired();
                e.Property(x => x.JoiningDate).HasColumnName("hire_date").IsRequired();
                e.Property(x => x.EmployeeCode).HasColumnName("employee_code").HasMaxLength(50); // controller will ensure non-empty (auto-generate) and uniqueness

                // Payroll / misc
                e.Property(x => x.BankDetails).HasColumnName("bank_details").IsRequired();
                e.Property(x => x.CustomFields).HasColumnName("custom_fields").IsRequired();
                e.Property(x => x.BenefitsEnrollment).HasColumnName("benefits_enrollment").IsRequired(false); // ← optional
                e.Property(x => x.ShiftDetails).HasColumnName("shift_details").IsRequired(false);             // ← optional

                // Timestamps
                e.Property(x => x.CreatedAt).HasColumnName("created_at")
                    .HasColumnType("datetime2(3)").HasDefaultValueSql("SYSUTCDATETIME()").IsRequired();
                e.Property(x => x.UpdatedAt).HasColumnName("updated_at")
                    .HasColumnType("datetime2(3)").HasDefaultValueSql("SYSUTCDATETIME()").IsRequired();
                e.Property(x => x.TerminatedDate).HasColumnName("terminated_date").HasColumnType("datetime2(3)");

                // JSON validity check (kept)
                e.ToTable(t => t.HasCheckConstraint("CHK_emp_custom_fields_json",
                    "custom_fields IS NULL OR ISJSON(custom_fields) = 1"));

                // Relationships

                // Tenant
                e.HasOne(x => x.Tenant)
                 .WithMany(t => t.Employees)
                 .HasForeignKey(x => x.TenantId)
                 .OnDelete(DeleteBehavior.Restrict);

                // Organization (tenant-safe composite)
                e.HasOne(x => x.Organization)
                 .WithMany(o => o.Employees)
                 .HasForeignKey(x => new { x.OrganizationId, x.TenantId })
                 .HasPrincipalKey(o => new { o.Id, o.TenantId })
                 .OnDelete(DeleteBehavior.Restrict);

                // Department (tenant-safe composite) — optional now
                e.HasOne(x => x.Department)
                 .WithMany(d => d.Employees)
                 .HasForeignKey(x => new { x.DepartmentId, x.OrganizationId, x.TenantId })
                 .HasPrincipalKey(d => new { d.Id, d.OrganizationId, d.TenantId })
                 .OnDelete(DeleteBehavior.Restrict);

                // Role
                e.HasOne(x => x.Role)
                 .WithMany() // you already map EmployeeRole separately
                 .HasForeignKey(x => x.RoleId)
                 .OnDelete(DeleteBehavior.Restrict);

                // Attendance (composite FK)
                e.HasMany(x => x.Attendances)
                 .WithOne(a => a.Employee)
                 .HasForeignKey(a => new { a.EmployeeId, a.TenantId })
                 .HasPrincipalKey(x => new { x.EmployeeID, x.TenantId })
                 .OnDelete(DeleteBehavior.Cascade);

                // Indexes / uniques
                e.HasIndex(x => new { x.TenantId, x.Email }).IsUnique();
                e.HasIndex(x => new { x.TenantId, x.EmployeeCode }).IsUnique()
                 .HasFilter("[employee_code] IS NOT NULL");

                e.HasIndex(x => new { x.TenantId, x.Username }).IsUnique(); // ← prevent duplicate usernames per tenant

                e.HasIndex(x => new { x.OrganizationId, x.TenantId });
                e.HasIndex(x => new { x.DepartmentId, x.OrganizationId, x.TenantId });
            });


            /* ===== ATTENDANCE (GUID + composite to Employee) ===== */
            model.Entity<Attendance>(a =>
            {
                a.ToTable("attendance");
                a.HasKey(x => x.Id);
                a.Property(x => x.Id).HasColumnName("Id").HasDefaultValueSql("NEWSEQUENTIALID()");

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

                // Composite FK -> Employee AK (EmployeeID, TenantId)
                a.HasOne(x => x.Employee)
                 .WithMany(e => e.Attendances)
                 .HasForeignKey(x => new { x.EmployeeId, x.TenantId })
                 .HasPrincipalKey(e => new { e.EmployeeID, e.TenantId })
                 .OnDelete(DeleteBehavior.Cascade);

                // Optional direct Tenant FK (keep if Attendance has Tenant nav)
                a.HasOne(x => x.Tenant)
                 .WithMany(t => t.Attendances)
                 .HasForeignKey(x => x.TenantId)
                 .OnDelete(DeleteBehavior.Restrict);

                a.HasIndex(x => new { x.EmployeeId, x.TenantId, x.AttendanceDate })
                 .HasDatabaseName("IX_attendance_employee_id_tenant_id_attendance_date");
                a.HasIndex(x => new { x.TenantId, x.AttendanceDate })
                 .HasDatabaseName("IX_attendance_tenant_id_attendance_date");
            });

            // ===== ROLES =====
            model.Entity<Role>(e =>
            {
                e.ToTable("roles");
                e.HasKey(r => r.Id);

                e.Property(r => r.Name).IsRequired().HasMaxLength(100);
                e.Property(r => r.Description).HasMaxLength(200);
                e.Property(r => r.PermissionsJson).HasColumnName("permissions"); // nvarchar(max) by default

                e.HasOne(r => r.Tenant)
                 .WithMany(t => t.Roles)
                 .HasForeignKey(r => r.TenantId)
                 .OnDelete(DeleteBehavior.Restrict);

                // Unique per tenant (NULL tenant means "system/global" set)
                e.HasIndex(r => new { r.TenantId, r.Name }).IsUnique();
            }); model.Entity<User>(e => e.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()"));
            model.Entity<Job>(e => e.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()"));
            model.Entity<Applicant>(e => e.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()"));
            model.Entity<Interview>(e => e.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()"));
            model.Entity<Leave>(e => e.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()"));
            model.Entity<LeaveType>(e => e.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()"));
            model.Entity<Goal>(e => e.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()"));
            model.Entity<PerformanceReview>(e => e.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()"));
            model.Entity<RequestFeedback>(e => e.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()"));
            model.Entity<Training>(e => e.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()"));
            model.Entity<TrainingEnrollment>(e => e.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()"));
            model.Entity<Asset>(e => e.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()"));
            model.Entity<TenantSetting>(e => e.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()"));
            model.Entity<OrgSetting>(e => e.Property(x => x.Id).HasDefaultValueSql("NEWSEQUENTIALID()"));

            // ===== EMPLOYEE ROLES =====
            model.Entity<EmployeeRole>(e =>
            {
                e.ToTable("employee_roles");
                e.HasKey(er => er.Id);

                e.HasOne(er => er.Employee)
                 .WithMany() // you can add ICollection<EmployeeRole> Roles on Employee if you want
                 .HasForeignKey(er => er.EmployeeId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(er => er.Role)
                 .WithMany(r => r.Members)
                 .HasForeignKey(er => er.RoleId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(er => er.Tenant)
                 .WithMany() // or .WithMany(t => t.EmployeeRoles) if you add nav on Tenant
                 .HasForeignKey(er => er.TenantId)
                 .OnDelete(DeleteBehavior.Restrict);

                // Prevent duplicate assignment of the same role for the same employee in the same tenant
                e.HasIndex(er => new { er.EmployeeId, er.RoleId, er.TenantId }).IsUnique();
            });

            // ===== SEED SYSTEM ROLES (global, TenantId = NULL) =====
            var ROLE_SUPERADMIN = Guid.Parse("00000000-0000-0000-0000-000000000001");
            var ROLE_TENANT_HR = Guid.Parse("00000000-0000-0000-0000-000000000002");
            var ROLE_EMPLOYEE = Guid.Parse("00000000-0000-0000-0000-000000000003");
            var ROLE_MANAGER = Guid.Parse("00000000-0000-0000-0000-000000000004");
            var ROLE_SUBMANAGER = Guid.Parse("00000000-0000-0000-0000-000000000005");

            // Keep permissions short & clear. You’ll check them in policies/handlers.
            model.Entity<Role>().HasData(
                new Role
                {
                    Id = ROLE_SUPERADMIN,
                    TenantId = null,
                    Name = "SuperAdmin",
                    Description = "Full cross-tenant access",
                    IsSystem = true,
                    PermissionsJson = "[\"*\"]" // wildcard = everything
                },
                new Role
                {
                    Id = ROLE_TENANT_HR,
                    TenantId = null,
                    Name = "HRAdmin",
                    Description = "Tenant-wide HR/admin; approve leave",
                    IsSystem = true,
                    PermissionsJson =
                        "[" +
                          "\"tenant.manage\"," +
                          "\"employees.read\",\"employees.create\",\"employees.update\",\"employees.delete\"," +
                          "\"departments.read\",\"departments.create\",\"departments.update\",\"departments.delete\"," +
                          "\"leave.read\",\"leave.approve\"," +
                          "\"attendance.read\",\"attendance.edit\"" +
                        "]"
                },
                new Role
                {
                    Id = ROLE_EMPLOYEE,
                    TenantId = null,
                    Name = "Employee",
                    Description = "Standard employee self-service",
                    IsSystem = true,
                    PermissionsJson =
                        "[" +
                          "\"self.read\",\"self.update\"," +
                          "\"attendance.clock\",\"attendance.read.self\"," +
                          "\"leave.request\",\"leave.read.self\"" +
                        "]"
                },
                new Role
                {
                    Id = ROLE_MANAGER,
                    TenantId = null,
                    Name = "Manager",
                    Description = "Manage team in same department; approve leave in dept",
                    IsSystem = true,
                    PermissionsJson =
                        "[" +
                          "\"employees.read.dept\",\"employees.update.dept\"," +
                          "\"leave.read.dept\",\"leave.approve.dept\"," +
                          "\"attendance.read.dept\"" +
                        "]"
                },
                new Role
                {
                    Id = ROLE_SUBMANAGER,
                    TenantId = null,
                    Name = "SubManager",
                    Description = "Read-only for team in dept; review leave",
                    IsSystem = true,
                    PermissionsJson =
                        "[" +
                          "\"employees.read.dept\"," +
                          "\"leave.read.dept\"," +
                          "\"attendance.read.dept\"" +
                        "]"
                }
            );

            /* ===== Applicants (JobId => GUID) ===== */
            model.Entity<Applicant>(e =>
            {
                e.ToTable("applicants");
                e.HasOne(a => a.Job)
                 .WithMany(j => j.Applicants)
                 .HasForeignKey(a => a.JobId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            /* ===== Leaves (uses Employee AK + optional Approver) ===== */
            model.Entity<Leave>(e =>
            {
                e.ToTable("leaves");
                e.HasKey(l => l.Id);

                // Columns
                e.Property(l => l.Id).HasColumnName("id");
                e.Property(l => l.EmployeeId).HasColumnName("employee_id");
                e.Property(l => l.TenantId).HasColumnName("tenant_id");
                e.Property(l => l.LeaveTypeId).HasColumnName("leave_type_id");
                e.Property(l => l.ApprovedBy).HasColumnName("approved_by"); // Guid? (nullable)

                e.Property(l => l.StartDate).HasColumnName("start_date").HasColumnType("date");
                e.Property(l => l.EndDate).HasColumnName("end_date").HasColumnType("date");
                e.Property(l => l.Status).HasColumnName("status").HasMaxLength(50);
                e.Property(l => l.Reason).HasColumnName("reason");
                e.Property(l => l.AppliedOn).HasColumnName("applied_on").HasColumnType("datetime2(3)");
                e.Property(l => l.ManagerComment).HasColumnName("manager_comment");

                // (employee_id, tenant_id) -> Employee (id, tenant_id)
                e.HasOne(l => l.Employee)
                 .WithMany()
                 .HasForeignKey(l => new { l.EmployeeId, l.TenantId })
                 .HasPrincipalKey(emp => new { emp.EmployeeID, emp.TenantId })
                 .OnDelete(DeleteBehavior.Restrict);

                // Approver (nullable) — enforce same-tenant by composite FK
                e.HasOne(l => l.Approver)
                 .WithMany()
                 .HasForeignKey(l => new { l.ApprovedBy, l.TenantId })
                 .HasPrincipalKey(emp => new { emp.EmployeeID, emp.TenantId })
                 .OnDelete(DeleteBehavior.Restrict);

                // LeaveType (Guid -> Guid)
                e.HasOne(l => l.LeaveType)
                 .WithMany(lt => lt.Leaves)
                 .HasForeignKey(l => l.LeaveTypeId)
                 .HasPrincipalKey(lt => lt.Id)
                 .OnDelete(DeleteBehavior.Restrict);

                // Tenant FK (optional nav collection on Tenant)
                e.HasOne(l => l.Tenant)
                 .WithMany()
                 .HasForeignKey(l => l.TenantId)
                 .OnDelete(DeleteBehavior.Restrict);

                // Helpful lookup index
                e.HasIndex(l => new { l.TenantId, l.EmployeeId, l.StartDate, l.EndDate });
            });


            /* ===== PerformanceReview (avoid multiple cascade paths) ===== */
            model.Entity<PerformanceReview>(e =>
            {
                e.ToTable("performance_reviews");
                e.HasOne(pr => pr.Employee)
                 .WithMany()
                 .HasForeignKey(pr => pr.EmployeeId)
                 .OnDelete(DeleteBehavior.NoAction);

                e.HasOne(pr => pr.Reviewer)
                 .WithMany()
                 .HasForeignKey(pr => pr.ReviewerId)
                 .OnDelete(DeleteBehavior.NoAction);
            });
            model.Entity<User>(e =>
            {
                e.ToTable("users");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id");
                e.Property(x => x.OrganizationId).HasColumnName("organization_id");
                e.HasOne(x => x.Organization)
                 .WithMany()
                 .HasForeignKey(x => x.OrganizationId)
                 .OnDelete(DeleteBehavior.Restrict);
            });


            
            // ===== ASSETS (GUID) =====
            model.Entity<Asset>(a =>
            {
                a.ToTable("assets");
                a.HasKey(x => x.Id);

                a.Property(x => x.Id)
                 .HasColumnName("id")
                 .HasDefaultValueSql("NEWSEQUENTIALID()");

                a.Property(x => x.OrganizationId).HasColumnName("organization_id");
                a.Property(x => x.TenantId).HasColumnName("tenant_id");
                a.Property(x => x.EmployeeId).HasColumnName("employee_id");

                a.Property(x => x.AssetName).HasColumnName("asset_name").IsRequired();
                a.Property(x => x.Status).HasColumnName("status").IsRequired();
                a.Property(x => x.IssuedOn).HasColumnName("issued_on");
                a.Property(x => x.ReturnedOn).HasColumnName("returned_on");
                a.Property(x => x.ConditionNotes).HasColumnName("condition_notes").IsRequired();
                a.Property(x => x.AssetTag).HasColumnName("asset_tag");
                a.Property(x => x.Category).HasColumnName("category").IsRequired();

                // (employee_id, tenant_id) -> Employee (id, tenant_id)
                a.HasOne(x => x.Employee)
                 .WithMany()
                 .HasForeignKey(x => new { x.EmployeeId, x.TenantId })
                 .HasPrincipalKey(e => new { e.EmployeeID, e.TenantId })
                 .OnDelete(DeleteBehavior.Restrict);

                // (organization_id, tenant_id) -> Organization (Id, TenantId) (org has AK on {Id,TenantId})
                a.HasOne(x => x.Organization)
                 .WithMany()
                 .HasForeignKey(x => new { x.OrganizationId, x.TenantId })
                 .HasPrincipalKey(o => new { o.Id, o.TenantId })
                 .OnDelete(DeleteBehavior.Restrict);

                // tenant
                a.HasOne(x => x.Tenant)
                 .WithMany()
                 .HasForeignKey(x => x.TenantId)
                 .OnDelete(DeleteBehavior.Restrict);

                a.HasIndex(x => new { x.TenantId, x.AssetTag })
                 .IsUnique()
                 .HasFilter("[asset_tag] IS NOT NULL");

                a.HasIndex(x => new { x.TenantId, x.Category });
            });


        }
    }
}