using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace MyExam.Backend.Models.DbMysqlModels;

public partial class OrderItemsContext : DbContext
{
    public OrderItemsContext()
    {
    }

    public OrderItemsContext(DbContextOptions<OrderItemsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<PizzaRendelesTetelek> PizzaRendelesTeteleks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;database=order_items;user=root", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.4.32-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8_hungarian_ci")
            .HasCharSet("utf8");

        modelBuilder.Entity<PizzaRendelesTetelek>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("pizza_rendeles_tetelek")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnType("int(2)")
                .HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasColumnType("int(1)")
                .HasColumnName("amount");
            entity.Property(e => e.Name)
                .HasMaxLength(16)
                .HasColumnName("name");
            entity.Property(e => e.OrderId)
                .HasColumnType("int(2)")
                .HasColumnName("orderId");
            entity.Property(e => e.Price)
                .HasColumnType("int(4)")
                .HasColumnName("price");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
