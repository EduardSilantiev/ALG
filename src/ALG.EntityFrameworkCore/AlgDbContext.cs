using ALG.Core.Services;
using ALG.Core.Users;
using Microsoft.EntityFrameworkCore;

namespace ALG.EntityFrameworkCore
{
    public class AlgDbContext : DbContext
    {
        public AlgDbContext() { }

        public AlgDbContext(DbContextOptions<AlgDbContext> options) : base(options) { }

        public virtual DbSet<ActivatedBonus> ActivatedBonuses { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Configure default schema (Ownership and User-Schema Separation)
            modelBuilder.HasDefaultSchema("onsi");

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ActivatedBonus>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.ServiceId })
                    .HasName("PK_UserService");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.ActivatedBonuses)
                    .HasForeignKey(d => d.ServiceId)
                    .HasConstraintName("FK_ActivatedBonus_Service");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ActivatedBonuses)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_ActivatedBonus_User");
            });

            modelBuilder.Entity<Service>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.HasIndex(e => e.Email).IsUnique();
            });
        }
    }
}