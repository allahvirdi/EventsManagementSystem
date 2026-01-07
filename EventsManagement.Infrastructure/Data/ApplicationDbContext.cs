using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EventsManagement.Shared.Entities;

namespace EventsManagement.Infrastructure.Data
{
    /// <summary>
    /// DbContext اصلی سامانه مدیریت رویدادها
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<AppUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSets برای تمام موجودیت‌ها
        public DbSet<DynamicTable> DynamicTables { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<School> Schools { get; set; }
        public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventTask> EventTasks { get; set; }
        public DbSet<TaskReply> TaskReplies { get; set; }
        public DbSet<EventRelationship> EventRelationships { get; set; }
        public DbSet<DocumentMetadata> DocumentMetadatas { get; set; }
        public DbSet<UserActivityLog> UserActivityLogs { get; set; }
        public DbSet<Comment> Comments { get; set; }

        /// <summary>
        /// تنظیم روابط و محدودیت‌های دیتابیس
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ============= AppUser تنظیمات =============
            modelBuilder.Entity<AppUser>()
                .HasOne(u => u.OrganizationUnit)
                .WithMany(ou => ou.Users)
                .HasForeignKey(u => u.OrganizationUnitId)
                .OnDelete(DeleteBehavior.SetNull);

            // ============= OrganizationUnit تنظیمات =============
            modelBuilder.Entity<OrganizationUnit>()
                .HasOne(ou => ou.ParentUnit)
                .WithMany(ou => ou.ChildUnits)
                .HasForeignKey(ou => ou.ParentUnitId)
                .OnDelete(DeleteBehavior.Restrict);

            // ============= Region تنظیمات =============
            modelBuilder.Entity<Region>()
                .HasOne(r => r.Province)
                .WithMany(p => p.Regions)
                .HasForeignKey(r => r.ProvinceId)
                .OnDelete(DeleteBehavior.Cascade);

            // ============= School تنظیمات =============
            modelBuilder.Entity<School>()
                .HasOne(s => s.Region)
                .WithMany(r => r.Schools)
                .HasForeignKey(s => s.RegionId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<School>()
                .HasOne(s => s.Province)
                .WithMany()
                .HasForeignKey(s => s.ProvinceId)
                .OnDelete(DeleteBehavior.SetNull);

            // ============= Event تنظیمات =============
            modelBuilder.Entity<Event>()
                .HasOne(e => e.ActionUnit)
                .WithMany()
                .HasForeignKey(e => e.ActionUnitId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Event>()
                .HasMany(e => e.Tasks)
                .WithOne(t => t.Event)
                .HasForeignKey(t => t.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Event>()
                .HasMany(e => e.Documents)
                .WithOne()
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Event>()
                .HasMany(e => e.Comments)
                .WithOne(c => c.Event)
                .HasForeignKey(c => c.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            // ============= EventTask تنظیمات =============
            modelBuilder.Entity<EventTask>()
                .HasOne(t => t.AssignedToUnit)
                .WithMany()
                .HasForeignKey(t => t.AssignedToUnitId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EventTask>()
                .HasMany(t => t.Replies)
                .WithOne(r => r.Task)
                .HasForeignKey(r => r.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EventTask>()
                .HasMany(t => t.Documents)
                .WithOne()
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            // ============= TaskReply تنظیمات =============
            modelBuilder.Entity<TaskReply>()
                .HasMany(r => r.Documents)
                .WithOne()
                .HasForeignKey(d => d.TaskReplyId)
                .OnDelete(DeleteBehavior.Cascade);

            // ============= EventRelationship تنظیمات =============
            modelBuilder.Entity<EventRelationship>()
                .HasOne(er => er.Event1)
                .WithMany(e => e.RelatedEvents)
                .HasForeignKey(er => er.EventId1)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EventRelationship>()
                .HasOne(er => er.Event2)
                .WithMany()
                .HasForeignKey(er => er.EventId2)
                .OnDelete(DeleteBehavior.Restrict);

            // ============= DynamicTable تنظیمات =============
            modelBuilder.Entity<DynamicTable>()
                .HasIndex(dt => new { dt.TableName, dt.Code })
                .IsUnique();

            // ============= Indexes تنظیمات =============
            modelBuilder.Entity<Event>()
                .HasIndex(e => e.CreatedAt);

            modelBuilder.Entity<EventTask>()
                .HasIndex(t => t.EventId);

            modelBuilder.Entity<UserActivityLog>()
                .HasIndex(ual => new { ual.UserId, ual.ActivityDateTime });

            // ============= String Properties =============
            modelBuilder.Entity<DynamicTable>()
                .Property(dt => dt.TableName)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<DynamicTable>()
                .Property(dt => dt.Value)
                .HasMaxLength(500)
                .IsRequired();

            modelBuilder.Entity<Province>()
                .Property(p => p.Name)
                .HasMaxLength(200)
                .IsRequired();

            modelBuilder.Entity<Region>()
                .Property(r => r.Name)
                .HasMaxLength(200)
                .IsRequired();

            modelBuilder.Entity<School>()
                .Property(s => s.Name)
                .HasMaxLength(200)
                .IsRequired();

            modelBuilder.Entity<OrganizationUnit>()
                .Property(ou => ou.Name)
                .HasMaxLength(200)
                .IsRequired();

            modelBuilder.Entity<Event>()
                .Property(e => e.Title)
                .HasMaxLength(500)
                .IsRequired();

            modelBuilder.Entity<EventTask>()
                .Property(t => t.Title)
                .HasMaxLength(500)
                .IsRequired();

            modelBuilder.Entity<DocumentMetadata>()
                .Property(d => d.FileName)
                .HasMaxLength(500)
                .IsRequired();

            modelBuilder.Entity<AppUser>()
                .Property(u => u.FullName)
                .HasMaxLength(200);
        }
    }
}
