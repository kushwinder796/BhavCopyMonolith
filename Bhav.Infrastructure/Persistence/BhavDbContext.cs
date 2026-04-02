using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Bhav.Infrastructure.Persistence.Entities;

public partial class BhavDbContext : DbContext
{
    public BhavDbContext()
    {
    }

    public BhavDbContext(DbContextOptions<BhavDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BhavCopy> BhavCopies { get; set; }

    public virtual DbSet<BhavCopyAudit> BhavCopyAudits { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=bhavCopy;Username=postgres;Password=root");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BhavCopy>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("bhav_copies_pkey");

            entity.ToTable("bhav_copies", "bhav");

            entity.HasIndex(e => new { e.Symbol, e.TradeDate }, "uq_bhav_symbol_date").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ClosePrice)
                .HasPrecision(10, 2)
                .HasColumnName("close_price");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasColumnName("created_by");
            entity.Property(e => e.HighPrice)
                .HasPrecision(10, 2)
                .HasColumnName("high_price");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.LowPrice)
                .HasPrecision(10, 2)
                .HasColumnName("low_price");
            entity.Property(e => e.OpenInterest)
                .HasPrecision(15, 2)
                .HasColumnName("open_interest");
            entity.Property(e => e.OpenPrice)
                .HasPrecision(10, 2)
                .HasColumnName("open_price");
            entity.Property(e => e.Symbol)
                .IsRequired()
                .HasMaxLength(10)
                .HasColumnName("symbol");
            entity.Property(e => e.TradeDate).HasColumnName("trade_date");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.Volume).HasColumnName("volume");
        });

        modelBuilder.Entity<BhavCopyAudit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("bhav_copy_audit_pkey");

            entity.ToTable("bhav_copy_audit", "bhav");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BhavCopyId).HasColumnName("bhav_copy_id");
            entity.Property(e => e.ChangedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("changed_at");
            entity.Property(e => e.ChangedBy)
                .HasMaxLength(100)
                .HasColumnName("changed_by");
            entity.Property(e => e.NewValues)
                .HasColumnType("json")
                .HasColumnName("new_values");
            entity.Property(e => e.OldValues)
                .HasColumnType("json")
                .HasColumnName("old_values");
            entity.Property(e => e.Operation)
                .HasMaxLength(20)
                .HasColumnName("operation");

            entity.HasOne(d => d.BhavCopy).WithMany(p => p.BhavCopyAudits)
                .HasForeignKey(d => d.BhavCopyId)
                .HasConstraintName("fk_bhav_audit");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
