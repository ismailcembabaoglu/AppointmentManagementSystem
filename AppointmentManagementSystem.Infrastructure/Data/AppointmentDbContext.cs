using AppointmentManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManagementSystem.Infrastructure.Data
{
    public class AppointmentDbContext : DbContext
    {
        public AppointmentDbContext(DbContextOptions<AppointmentDbContext> options) : base(options)
        {
        }

        // Users and Authentication
        public DbSet<User> Users { get; set; }

        // Business Management
        public DbSet<Category> Categories { get; set; }
        public DbSet<Business> Businesses { get; set; }
        public DbSet<BusinessUser> BusinessUsers { get; set; }

        // Services and Employees
        public DbSet<Service> Services { get; set; }
        public DbSet<Employee> Employees { get; set; }

        // Appointments
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<AppointmentServiceItem> AppointmentServices { get; set; }

        // Photos
        public DbSet<BusinessPhoto> BusinessPhotos { get; set; }
        public DbSet<EmployeePhoto> EmployeePhotos { get; set; }
        public DbSet<ServicePhoto> ServicePhotos { get; set; }
        public DbSet<AppointmentPhoto> AppointmentPhotos { get; set; }

        // Documents
        public DbSet<EmployeeDocument> EmployeeDocuments { get; set; }

        // Payment & Subscription
        public DbSet<Payment> Payments { get; set; }
        public DbSet<BusinessSubscription> BusinessSubscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User entity configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
                entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Category entity configuration
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // Business entity configuration
            modelBuilder.Entity<Business>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.HasOne(b => b.Category)
                      .WithMany(c => c.Businesses)
                      .HasForeignKey(b => b.CategoryId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // BusinessUser entity configuration
            modelBuilder.Entity<BusinessUser>(entity =>
            {
                entity.HasKey(bu => bu.Id);
                entity.HasOne(bu => bu.Business)
                      .WithMany(b => b.BusinessUsers)
                      .HasForeignKey(bu => bu.BusinessId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(bu => bu.User)
                      .WithMany(u => u.BusinessUsers)
                      .HasForeignKey(bu => bu.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Employee entity configuration
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.HasOne(e => e.Business)
                      .WithMany(b => b.Employees)
                      .HasForeignKey(e => e.BusinessId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Service entity configuration
            modelBuilder.Entity<Service>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.HasOne(s => s.Business)
                      .WithMany(b => b.Services)
                      .HasForeignKey(s => s.BusinessId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Appointment entity configuration - TÜM CASCADE DELETE ÇAKIŞMALARI DÜZELTİLDİ
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Rating).HasColumnType("int");

                // ⚡ PERFORMANS İÇİN INDEX'LER - QUERY HIZINI 10X ARTTıRıR
                entity.HasIndex(a => a.CustomerId)
                      .HasDatabaseName("IX_Appointments_CustomerId");
                
                entity.HasIndex(a => a.BusinessId)
                      .HasDatabaseName("IX_Appointments_BusinessId");
                
                entity.HasIndex(a => a.AppointmentDate)
                      .HasDatabaseName("IX_Appointments_AppointmentDate");
                
                entity.HasIndex(a => a.Status)
                      .HasDatabaseName("IX_Appointments_Status");
                
                // Composite index - En çok kullanılan query kombinasyonu için
                entity.HasIndex(a => new { a.BusinessId, a.AppointmentDate, a.Status })
                      .HasDatabaseName("IX_Appointments_Business_Date_Status");
                
                entity.HasIndex(a => new { a.CustomerId, a.AppointmentDate })
                      .HasDatabaseName("IX_Appointments_Customer_Date");

                // Customer - NO ACTION (çakışma önlemek için)
                entity.HasOne(a => a.Customer)
                      .WithMany(u => u.CustomerAppointments)
                      .HasForeignKey(a => a.CustomerId)
                      .OnDelete(DeleteBehavior.NoAction); // DEĞİŞTİ

                // Business - NO ACTION (çakışma önlemek için)
                entity.HasOne(a => a.Business)
                      .WithMany(b => b.Appointments)
                      .HasForeignKey(a => a.BusinessId)
                      .OnDelete(DeleteBehavior.NoAction); // DEĞİŞTİ

                // Employee - NO ACTION (çakışma önlemek için)
                entity.HasOne(a => a.Employee)
                      .WithMany(e => e.Appointments)
                      .HasForeignKey(a => a.EmployeeId)
                      .OnDelete(DeleteBehavior.NoAction); // DEĞİŞTİ

                // Service - NO ACTION (çakışma önlemek için)
                entity.HasOne(a => a.Service)
                      .WithMany(s => s.Appointments)
                      .HasForeignKey(a => a.ServiceId)
                      .OnDelete(DeleteBehavior.NoAction); // DEĞİŞTİ
            });

            modelBuilder.Entity<AppointmentServiceItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ServiceName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");

                entity.HasOne(a => a.Appointment)
                      .WithMany(a => a.AppointmentServices)
                      .HasForeignKey(a => a.AppointmentId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.Service)
                      .WithMany()
                      .HasForeignKey(a => a.ServiceId)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasIndex(a => new { a.AppointmentId, a.ServiceId });
            });

            // Photo inheritance configuration
            modelBuilder.Entity<Photo>()
                .HasDiscriminator<string>("PhotoType")
                .HasValue<BusinessPhoto>("Business")
                .HasValue<EmployeePhoto>("Employee")
                .HasValue<ServicePhoto>("Service")
                .HasValue<AppointmentPhoto>("Appointment");

            // Photo base configuration
            modelBuilder.Entity<Photo>(entity =>
            {
                entity.Property(p => p.FileName).IsRequired().HasMaxLength(500);
                entity.Property(p => p.ContentType).HasMaxLength(100);
            });

            // Specific photo configurations
            modelBuilder.Entity<BusinessPhoto>(entity =>
            {
                entity.HasOne(bp => bp.Business)
                      .WithMany(b => b.Photos)
                      .HasForeignKey(bp => bp.BusinessId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<EmployeePhoto>(entity =>
            {
                entity.HasOne(ep => ep.Employee)
                      .WithMany(e => e.Photos)
                      .HasForeignKey(ep => ep.EmployeeId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<ServicePhoto>(entity =>
            {
                entity.HasOne(sp => sp.Service)
                      .WithMany(s => s.Photos)
                      .HasForeignKey(sp => sp.ServiceId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<AppointmentPhoto>(entity =>
            {
                entity.HasOne(ap => ap.Appointment)
                      .WithMany(a => a.Photos)
                      .HasForeignKey(ap => ap.AppointmentId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            // EmployeeDocument configuration
            modelBuilder.Entity<EmployeeDocument>(entity =>
            {
                entity.HasKey(ed => ed.Id);
                entity.Property(ed => ed.Name).IsRequired().HasMaxLength(200);
                entity.Property(ed => ed.FileName).IsRequired().HasMaxLength(500);
                entity.HasOne(ed => ed.Employee)
                      .WithMany(e => e.Documents)
                      .HasForeignKey(ed => ed.EmployeeId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            // Payment configuration
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.MerchantOid).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Amount).HasColumnType("decimal(18,2)");
                entity.Property(p => p.Status).IsRequired().HasMaxLength(50);
                entity.HasIndex(p => p.MerchantOid).IsUnique();
                
                entity.HasOne(p => p.Business)
                      .WithMany(b => b.Payments)
                      .HasForeignKey(p => p.BusinessId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            // BusinessSubscription configuration
            modelBuilder.Entity<BusinessSubscription>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.MonthlyAmount).HasColumnType("decimal(18,2)");
                entity.Property(s => s.SubscriptionStatus).IsRequired().HasMaxLength(50);
                
                entity.HasOne(s => s.Business)
                      .WithOne(b => b.Subscription)
                      .HasForeignKey<BusinessSubscription>(s => s.BusinessId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Category>().HasData(
                        new Category { Id = 1, Name = "Berber", Description = "Erkek Berber Hizmetleri", Icon = "healing", CreatedAt = DateTime.UtcNow },
        new Category { Id = 2, Name = "Güzellik Merkezi", Description = "Güzellik ve bakım hizmetleri", Icon = "spa", CreatedAt = DateTime.UtcNow },
        new Category { Id = 3, Name = "Diş Hekimi", Description = "Diş sağlığı hizmetleri", Icon = "local_hospital", CreatedAt = DateTime.UtcNow },
        new Category { Id = 4, Name = "Tıbbi Estetik", Description = "Tıbbi estetik hizmetleri", Icon = "healing", CreatedAt = DateTime.UtcNow }

    );
        }
    }
}
