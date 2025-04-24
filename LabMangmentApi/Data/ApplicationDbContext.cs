using LabMangmentApi.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LabMangmentApi.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Maintenance> Maintenances { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Alert> Alerts { get; set; }
        public DbSet<AlertRecipient> AlertRecipients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // AlertRecipient (many-to-many بين User و Alert)
            modelBuilder.Entity<AlertRecipient>()
                .HasKey(ar => new { ar.UserId, ar.AlertId });

            modelBuilder.Entity<AlertRecipient>()
                .HasOne(ar => ar.User)
                .WithMany(u => u.AlertRecipients)
                .HasForeignKey(ar => ar.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AlertRecipient>()
                .HasOne(ar => ar.Alert)
                .WithMany(a => a.AlertRecipients)
                .HasForeignKey(ar => ar.AlertId)
                .OnDelete(DeleteBehavior.NoAction);

            // Maintenance -> User
            modelBuilder.Entity<Maintenance>()
                .HasOne(m => m.user)
                .WithMany(u => u.Maintenances)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Maintenance -> Device
            modelBuilder.Entity<Maintenance>()
                .HasOne(m => m.Device)
                .WithMany(d => d.MaintenanceReports)
                .HasForeignKey(m => m.DeviceId)
                .OnDelete(DeleteBehavior.NoAction);

            // Reservation -> User
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Reservation -> Device
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Device)
                .WithMany(d => d.Reservations)
                .HasForeignKey(r => r.DeviceId)
                .OnDelete(DeleteBehavior.NoAction);

            // Alert -> Device
            modelBuilder.Entity<Alert>()
                .HasOne(a => a.Device)
                .WithMany(d => d.Alerts)
                .HasForeignKey(a => a.DeviceId)
                .OnDelete(DeleteBehavior.NoAction);

            // User -> ApplicationUser           
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(a => a.User)
                .WithOne(u => u.ApplicationUser)
                .HasForeignKey<User>(u => u.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade); // 🟢 Cascade delete is ON

            // Seed Roles
            var studentId = "0c20a355-12dd-449d-a8d5-6e33960c45ee";
            var professorId = "393f1091-b175-4cca-a1df-19971e86e2a3";
            var technicalId = "7d090697-295a-43bf-bb0b-3a19843fb528";

            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = studentId,
                    ConcurrencyStamp = studentId,
                    Name = "Student",
                    NormalizedName = "Student".ToUpper()
                },
                new IdentityRole
                {
                    Id = professorId,
                    ConcurrencyStamp = professorId,
                    Name = "Professor",
                    NormalizedName = "Professor".ToUpper()
                },
                new IdentityRole
                {
                    Id = technicalId,
                    ConcurrencyStamp = technicalId,
                    Name = "Technical",
                    NormalizedName = "Technical".ToUpper()
                }
            };

            modelBuilder.Entity<IdentityRole>().HasData(roles);

        }
    }
}
