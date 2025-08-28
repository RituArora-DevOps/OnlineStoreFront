using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace OnlineStoreFront.Models.Business;

public partial class OnlineStoreContext : DbContext
{
    public OnlineStoreContext()
    {
    }

    public OnlineStoreContext(DbContextOptions<OnlineStoreContext> options)
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
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-ACG7IVF;Database=OnlineStore;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PK__Carts__51BCD7B79EE15A13");

            entity.ToTable("Carts", "Cart", tb => tb.HasTrigger("trg_carts_modified"));

            entity.HasIndex(e => e.ExternalUserId, "IX_Carts_ExternalUserId");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.ExternalUserId).HasMaxLength(128);
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(NULL)");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.CartItemId).HasName("PK__CartItem__488B0B0AD6FDE116");

            entity.ToTable("CartItems", "Cart", tb => tb.HasTrigger("trg_cartitems_modified"));

            entity.HasIndex(e => e.CartId, "IX_CartItems_CartId");

            entity.HasIndex(e => e.ProductId, "IX_CartItems_ProductId");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(NULL)");
            entity.Property(e => e.Quantity).HasDefaultValue(1);

            entity.HasOne(d => d.Cart).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.CartId)
                .HasConstraintName("FK__CartItems__CartI__5535A963");

            entity.HasOne(d => d.Product).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CartItems__Produ__5629CD9C");
        });

        modelBuilder.Entity<Clothing>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Clothing__B40CC6CD6AB124FC");

            entity.ToTable("Clothing", "Product");

            entity.Property(e => e.ProductId).ValueGeneratedNever();
            entity.Property(e => e.Color).HasMaxLength(50);
            entity.Property(e => e.Size).HasMaxLength(20);

            entity.HasOne(d => d.Product).WithOne(p => p.Clothing)
                .HasForeignKey<Clothing>(d => d.ProductId)
                .HasConstraintName("FK__Clothing__Produc__4222D4EF");
        });

        modelBuilder.Entity<CreditCardPayment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__CreditCa__9B556A3883370F97");

            entity.ToTable("CreditCardPayments", "Payment");

            entity.Property(e => e.PaymentId).ValueGeneratedNever();
            entity.Property(e => e.CardBrand).HasMaxLength(20);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Last4).HasMaxLength(4);

            entity.HasOne(d => d.Payment).WithOne(p => p.CreditCardPayment)
                .HasForeignKey<CreditCardPayment>(d => d.PaymentId)
                .HasConstraintName("FK__CreditCar__Payme__5DCAEF64");
        });

        modelBuilder.Entity<Electronic>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Electron__B40CC6CD64AF28E6");

            entity.ToTable("Electronics", "Product");

            entity.Property(e => e.ProductId).ValueGeneratedNever();
            entity.Property(e => e.WarrantyPeriod).HasDefaultValue(12);

            entity.HasOne(d => d.Product).WithOne(p => p.Electronic)
                .HasForeignKey<Electronic>(d => d.ProductId)
                .HasConstraintName("FK__Electroni__Produ__3C69FB99");
        });

        modelBuilder.Entity<Grocery>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Grocery__B40CC6CD6A225F38");

            entity.ToTable("Grocery", "Product");

            entity.Property(e => e.ProductId).ValueGeneratedNever();

            entity.HasOne(d => d.Product).WithOne(p => p.Grocery)
                .HasForeignKey<Grocery>(d => d.ProductId)
                .HasConstraintName("FK__Grocery__Product__3F466844");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BCF80898D5D");

            entity.ToTable("Orders", "Order", tb => tb.HasTrigger("trg_orders_modified"));

            entity.HasIndex(e => e.ExternalUserId, "IX_Orders_ExternalUserId");

            entity.HasIndex(e => e.OrderStatusId, "IX_Orders_OrderStatusId");

            entity.HasIndex(e => e.PaymentId, "IX_Orders_PaymentId");

            entity.HasIndex(e => new { e.ExternalUserId, e.OrderStatusId }, "IX_Orders_User_Status");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.ExternalUserId).HasMaxLength(128);
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(NULL)");
            entity.Property(e => e.OrderStatusId).HasDefaultValue(1);

            entity.HasOne(d => d.OrderStatus).WithMany(p => p.Orders)
                .HasForeignKey(d => d.OrderStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_OrderStatus");

            entity.HasOne(d => d.Payment).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PaymentId)
                .HasConstraintName("FK_Orders_Payment");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId).HasName("PK__OrderIte__57ED0681EFCA0C0F");

            entity.ToTable("OrderItems", "Order", tb => tb.HasTrigger("trg_orderitems_modified"));

            entity.HasIndex(e => e.OrderId, "IX_OrderItems_OrderId");

            entity.HasIndex(e => e.ProductId, "IX_OrderItems_ProductId");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(NULL)");
            entity.Property(e => e.PriceAtOrder).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Quantity).HasDefaultValue(1);

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__OrderItem__Order__70DDC3D8");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderItem__Produ__71D1E811");
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.HasKey(e => e.OrderStatusId).HasName("PK__OrderSta__BC674CA113751685");

            entity.ToTable("OrderStatus", "Order");

            entity.HasIndex(e => e.Status, "UQ__OrderSta__3A15923FEC3A231E").IsUnique();

            entity.Property(e => e.Status).HasMaxLength(20);
        });

        modelBuilder.Entity<PayPalPayment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__PayPalPa__9B556A38D6BFAE3F");

            entity.ToTable("PayPalPayments", "Payment");

            entity.Property(e => e.PaymentId).ValueGeneratedNever();
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.PayPalEmail).HasMaxLength(100);

            entity.HasOne(d => d.Payment).WithOne(p => p.PayPalPayment)
                .HasForeignKey<PayPalPayment>(d => d.PaymentId)
                .HasConstraintName("FK__PayPalPay__Payme__619B8048");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__9B556A381140AC04");

            entity.ToTable("Payments", "Payment", tb => tb.HasTrigger("trg_payments_modified"));

            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(NULL)");
        });

        modelBuilder.Entity<Picture>(entity =>
        {
            entity.HasKey(e => e.PictureId).HasName("PK__Pictures__8C2866D834B48DD7");

            entity.ToTable("Pictures", "Product");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(NULL)");
            entity.Property(e => e.PicFileName).HasMaxLength(100);
            entity.Property(e => e.PictureName).HasMaxLength(40);

            entity.HasOne(d => d.Product).WithMany(p => p.Pictures)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Pictures__Produc__46E78A0C");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__B40CC6CDF6FC7C44");

            entity.ToTable("Products", "Product", tb => tb.HasTrigger("trg_products_modified"));

            entity.HasIndex(e => e.Category, "IX_Products_Category");

            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(NULL)");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<ProductReview>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__ProductR__74BC79CEF5A586BE");

            entity.ToTable("ProductReviews", "Product", tb => tb.HasTrigger("trg_productreviews_modified"));

            entity.HasIndex(e => e.ExternalUserId, "IX_ProductReviews_ExternalUserId");

            entity.HasIndex(e => e.ProductId, "IX_ProductReviews_ProductId");

            entity.Property(e => e.Comment).HasMaxLength(1000);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.ExternalUserId).HasMaxLength(128);
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(NULL)");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductReviews)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductRe__Produ__4BAC3F29");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
