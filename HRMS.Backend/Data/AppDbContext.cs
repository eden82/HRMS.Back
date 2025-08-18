using Microsoft.EntityFrameworkCore;
using HRMS.Backend.Models;


namespace HRMS.Backend.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Role> Roles { get; set; }
		public DbSet<User> Users { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Attendance> Attendances { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);


            //modelBuilder.Entity<Tenant>()
            //    .HasMany(t => t.Users)
            //    .WithOne(u => u.Tenant)
            //    .HasForeignKey(u => u.TenantId)
            //    .OnDelete(DeleteBehavior.Restrict); // <- Prevents cascade path issue

            modelBuilder.Entity<Tenant>()
                .HasMany(t => t.Roles)
                .WithOne(r => r.Tenant)
                .HasForeignKey(r => r.TenantId)
                .OnDelete(DeleteBehavior.Cascade); // <- Allow cascade here
            // Organization to Employee relationship
            modelBuilder.Entity<Organization>()
                .HasMany(o => o.Departments)
                .WithOne(d => d.Organization)
                .HasForeignKey(d => d.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

           

            modelBuilder.Entity<Department>()
                .HasMany(d => d.Employees)
                .WithOne(e => e.Department)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Employee>()
                .HasMany(d => d.Attendances)
                .WithOne(e => e.Employee)
                .HasForeignKey(e => e.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);



            modelBuilder.Entity<User>()
                 .HasOne<Organization>()
                 .WithMany() // no Users collection
                 .HasForeignKey(u => u.OrganizationId)
                 .OnDelete(DeleteBehavior.Cascade); // <-- cascade delete




            modelBuilder.Entity<Tenant>()
                .HasMany(t => t.Organizations)
                .WithOne(o => o.Tenant)
                .HasForeignKey(o => o.TenantId)
                .OnDelete(DeleteBehavior.Cascade);



            modelBuilder.Entity<Tenant>()
                .Property(t => t.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            


        }




    }
}
