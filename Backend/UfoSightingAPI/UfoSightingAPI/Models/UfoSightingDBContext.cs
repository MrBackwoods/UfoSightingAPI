using Microsoft.EntityFrameworkCore;

namespace UfoSightingAPI.Models;

public partial class UfoSightingDBContext : DbContext
{
    public UfoSightingDBContext()
    {
    }

    public UfoSightingDBContext(DbContextOptions<UfoSightingDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Member> Member { get; set; }

    public virtual DbSet<Sighting> Sighting { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AS");

        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.MemberId).HasName("PK__Member__0CF04B388A616D79");

            entity.HasIndex(e => e.ApiKey, "UQ__Member__A4E6E1860CC4DC50").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Member__A9D1053403E65B4D").IsUnique();

            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.ApiKey).HasMaxLength(255);
            entity.Property(e => e.ApiKeyActivationDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ApiKeyDeactivationDate).HasDefaultValueSql("(dateadd(year,(1),getdate()))");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IsAdmin).HasDefaultValue(false);
            entity.Property(e => e.JoinDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Sighting>(entity =>
        {
            entity.HasKey(e => e.SightingId).HasName("PK__Sighting__C88E50A05B7944AB");

            entity.Property(e => e.SightingId).HasColumnName("SightingID");
            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.Latitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Occurred).HasColumnType("datetime");
            entity.Property(e => e.Reported)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
