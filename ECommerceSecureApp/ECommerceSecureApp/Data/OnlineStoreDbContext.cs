using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ECommerceSecureApp.Models;

public partial class OnlineStoreDbContext : DbContext
{
    public OnlineStoreDbContext()
    {
    }

    public OnlineStoreDbContext(DbContextOptions<OnlineStoreDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<Clothing> Clothings { get; set; }

    public virtual DbSet<CreditCardPayment> CreditCardPayments { get; set; }

    public virtual DbSet<Electronic> Electronics { get; set; }

    public virtual DbSet<Grocery> Groceries { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

    public virtual DbSet<PayPalPayment> PayPalPayments { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Picture> Pictures { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductReview> ProductReviews { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }       

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PK__Carts__51BCD7B79EE15A13");

            entity.ToTable("Carts", "Cart", tb => tb.HasTrigger("trg_carts_modified"));

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(NULL)");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.CartItemId).HasName("PK__CartItem__488B0B0AD6FDE116");

            entity.ToTable("CartItems", "Cart", tb => tb.HasTrigger("trg_cartitems_modified"));

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(NULL)");
            entity.Property(e => e.Quantity).HasDefaultValue(1);

            entity.HasOne(d => d.Cart).WithMany(p => p.CartItems).HasConstraintName("FK__CartItems__CartI__5535A963");

            entity.HasOne(d => d.Product).WithMany(p => p.CartItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CartItems__Produ__5629CD9C");
        });

        modelBuilder.Entity<Clothing>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Clothing__B40CC6CD6AB124FC");

            entity.Property(e => e.ProductId).ValueGeneratedNever();

            entity.HasOne(d => d.Product).WithOne(p => p.Clothing).HasConstraintName("FK__Clothing__Produc__4222D4EF");
        });

        modelBuilder.Entity<CreditCardPayment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__CreditCa__9B556A3883370F97");

            entity.Property(e => e.PaymentId).ValueGeneratedNever();
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.Payment).WithOne(p => p.CreditCardPayment).HasConstraintName("FK__CreditCar__Payme__5DCAEF64");
        });

        modelBuilder.Entity<Electronic>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Electron__B40CC6CD64AF28E6");

            entity.Property(e => e.ProductId).ValueGeneratedNever();
            entity.Property(e => e.WarrantyPeriod).HasDefaultValue(12);

            entity.HasOne(d => d.Product).WithOne(p => p.Electronic).HasConstraintName("FK__Electroni__Produ__3C69FB99");
        });

        modelBuilder.Entity<Grocery>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Grocery__B40CC6CD6A225F38");

            entity.Property(e => e.ProductId).ValueGeneratedNever();

            entity.HasOne(d => d.Product).WithOne(p => p.Grocery).HasConstraintName("FK__Grocery__Product__3F466844");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BCF80898D5D");

            entity.ToTable("Orders", "Order", tb => tb.HasTrigger("trg_orders_modified"));

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(NULL)");
            entity.Property(e => e.OrderStatusId).HasDefaultValue(1);

            entity.HasOne(d => d.OrderStatus).WithMany(p => p.Orders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_OrderStatus");

            entity.HasOne(d => d.Payment).WithMany(p => p.Orders).HasConstraintName("FK_Orders_Payment");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId).HasName("PK__OrderIte__57ED0681EFCA0C0F");

            entity.ToTable("OrderItems", "Order", tb => tb.HasTrigger("trg_orderitems_modified"));

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(NULL)");
            entity.Property(e => e.Quantity).HasDefaultValue(1);

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems).HasConstraintName("FK__OrderItem__Order__70DDC3D8");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderItem__Produ__71D1E811");
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.HasKey(e => e.OrderStatusId).HasName("PK__OrderSta__BC674CA113751685");
        });

        modelBuilder.Entity<PayPalPayment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__PayPalPa__9B556A38D6BFAE3F");

            entity.Property(e => e.PaymentId).ValueGeneratedNever();
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.Payment).WithOne(p => p.PayPalPayment).HasConstraintName("FK__PayPalPay__Payme__619B8048");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__9B556A381140AC04");

            entity.ToTable("Payments", "Payment", tb => tb.HasTrigger("trg_payments_modified"));

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(NULL)");
        });

        modelBuilder.Entity<Picture>(entity =>
        {
            entity.HasKey(e => e.PictureId).HasName("PK__Pictures__8C2866D834B48DD7");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(NULL)");

            entity.HasOne(d => d.Product).WithMany(p => p.Pictures).HasConstraintName("FK__Pictures__Produc__46E78A0C");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__B40CC6CDF6FC7C44");

            entity.ToTable("Products", "Product", tb => tb.HasTrigger("trg_products_modified"));

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(NULL)");
        });

        modelBuilder.Entity<ProductReview>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__ProductR__74BC79CEF5A586BE");

            entity.ToTable("ProductReviews", "Product", tb => tb.HasTrigger("trg_productreviews_modified"));

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(NULL)");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductReviews)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductRe__Produ__4BAC3F29");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
