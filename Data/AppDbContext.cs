using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SE1StudentTracker.Models;
using System.Security;

namespace SE1StudentTracker.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser, IdentityRole, String>
    {

        private readonly IConfiguration _configuration;
        public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        // Add non-Identity tables. Uneccessary tables have been commented out but left in "just in case".
        //public DbSet<Role> Roles { get; set; }
        //public DbSet<UserAccount> UserAccounts { get; set; }
        //public DbSet<StudentProfile> StudentProfiles { get; set; }
        //public DbSet<InstructorProfile> InstructorProfiles { get; set; }
        //public DbSet<Course> Courses { get; set; }
        //public DbSet<Section> Sections { get; set; }
        //public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<TimeSession> time_session { get; set; }
        //public DbSet<AuditLog> AuditLogs { get; set; }
        //public DbSet<Permission> Permissions { get; set; }
        //public DbSet<RolePermission> RolePermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);



            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(bool))
                    {
                        property.SetColumnType("NUMBER(1)");
                    }
                }
            }
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            // SQLite connection needs this pragma enabled
            using (var db = new SqliteConnection(connectionString))
            {
                db.Open();
                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandText = "PRAGMA foreign_keys = ON;";
                    cmd.ExecuteNonQuery();
                }
            }
        }

    }

}
