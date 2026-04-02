using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Margin.Infrastructure.Persistence.Entities;

public partial class MarginDbContext : DbContext
{
    public MarginDbContext()
    {
    }

    public MarginDbContext(DbContextOptions<MarginDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<MarginDetail> MarginDetails { get; set; }

    public virtual DbSet<MarginHistory> MarginHistories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=bhavCopy;Username=postgres;Password=root");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MarginDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("margin_details_pkey");

            entity.ToTable("margin_details", "margin");

            entity.HasIndex(e => new { e.Symbol, e.TradeDate }, "uq_margin_symbol_date").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.ExtremeMargin)
                .HasPrecision(10, 2)
                .HasColumnName("extreme_margin");
            entity.Property(e => e.InitialMargin)
                .HasPrecision(10, 2)
                .HasColumnName("initial_margin");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.MarginPercentage)
                .HasPrecision(10, 4)
                .HasColumnName("margin_percentage");
            entity.Property(e => e.Symbol)
                .IsRequired()
                .HasMaxLength(10)
                .HasColumnName("symbol");
            entity.Property(e => e.TotalMargin)
                .HasPrecision(10, 2)
                .HasColumnName("total_margin");
            entity.Property(e => e.TradeDate).HasColumnName("trade_date");
            entity.Property(e => e.VarMargin)
                .HasPrecision(10, 2)
                .HasColumnName("var_margin");
        });

        modelBuilder.Entity<MarginHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("margin_history_pkey");

            entity.ToTable("margin_history", "margin");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.MarginId).HasColumnName("margin_id");
            entity.Property(e => e.RecordedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("recorded_at");
            entity.Property(e => e.Symbol)
                .HasMaxLength(10)
                .HasColumnName("symbol");
            entity.Property(e => e.TotalMargin)
                .HasPrecision(10, 2)
                .HasColumnName("total_margin");
            entity.Property(e => e.TradeDate).HasColumnName("trade_date");

            entity.HasOne(d => d.Margin).WithMany(p => p.MarginHistories)
                .HasForeignKey(d => d.MarginId)
                .HasConstraintName("fk_margin_history");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
