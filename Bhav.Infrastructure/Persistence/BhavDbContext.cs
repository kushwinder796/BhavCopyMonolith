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

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Data)
                .HasColumnType("jsonb")
                .HasColumnName("data");
            entity.Property(e => e.Exchange)
                .HasMaxLength(10)
                .HasColumnName("exchange");
            entity.Property(e => e.Segment)
                .HasMaxLength(10)
                .HasColumnName("segment");
            entity.Property(e => e.Symbol)
                .HasMaxLength(50)
                .HasColumnName("symbol");
            entity.Property(e => e.TradeDate).HasColumnName("trade_date");
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
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
