using System;
using Microsoft.EntityFrameworkCore;
using OptimizingLastMile.Entites;

namespace OptimizingLastMile.Configs;

public class OlmDbContext : DbContext
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<OrderInformation> OrderInformation { get; set; }
    public DbSet<NotificationLog> NotificationLogs { get; set; }

    public OlmDbContext(DbContextOptions<OlmDbContext> dbContextOptions) : base(dbContextOptions)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Account
        modelBuilder.Entity<Account>(accountBuilder =>
        {
            accountBuilder.ToTable(nameof(Account));

            accountBuilder.HasKey(a => a.Id);

            accountBuilder.Property(a => a.PhoneNumber).HasMaxLength(11).IsRequired(false);
            accountBuilder.Property(a => a.Email).HasMaxLength(500).IsRequired(false);
            accountBuilder.Property(a => a.Username).HasMaxLength(50).IsRequired(false);
            accountBuilder.Property(a => a.Password).IsRequired(false);

            accountBuilder.Ignore(a => a.CountOrderShipping);
            accountBuilder.Ignore(a => a.CountOrderCreatedShipping);
            accountBuilder.Ignore(a => a.CountOwnershipOrderInProcess);
        });

        // AccountProfile
        modelBuilder.Entity<AccountProfile>(apBuilder =>
        {
            apBuilder.ToTable(nameof(AccountProfile));

            apBuilder.HasKey(a => a.Id);

            apBuilder.HasOne<Account>().WithOne(account => account.AccountProfile).HasForeignKey<AccountProfile>(a => a.Id);

            apBuilder.Property(a => a.Name).HasMaxLength(200);
            apBuilder.Property(a => a.BirthDay).HasColumnType("date").IsRequired(false);
            apBuilder.Property(a => a.Province).HasMaxLength(200).IsRequired(false);
            apBuilder.Property(a => a.District).HasMaxLength(200).IsRequired(false);
            apBuilder.Property(a => a.Ward).HasMaxLength(200).IsRequired(false);
            apBuilder.Property(a => a.Address).HasMaxLength(200).IsRequired(false);
            apBuilder.Property(a => a.PhoneContact).HasMaxLength(11).IsRequired(false);
        });

        // Device
        modelBuilder.Entity<Device>(deviceBuilder =>
        {
            deviceBuilder.ToTable(nameof(Device));

            deviceBuilder.HasKey(d => d.Id);

            deviceBuilder.HasOne<Account>().WithMany().HasForeignKey(d => d.AccountId);
        });

        // DriverProfile
        modelBuilder.Entity<DriverProfile>(dpBuilder =>
        {
            dpBuilder.ToTable(nameof(DriverProfile));

            dpBuilder.HasKey(d => d.Id);

            dpBuilder.HasOne<Account>().WithOne(account => account.DriverProfile).HasForeignKey<DriverProfile>(d => d.Id);

            dpBuilder.Property(d => d.Name).HasMaxLength(200);
            dpBuilder.Property(d => d.BirthDay).HasColumnType("date");
            dpBuilder.Property(d => d.Province).HasMaxLength(200);
            dpBuilder.Property(d => d.District).HasMaxLength(200);
            dpBuilder.Property(d => d.Ward).HasMaxLength(200);
            dpBuilder.Property(d => d.Address).HasMaxLength(200);
            dpBuilder.Property(d => d.PhoneContact).HasMaxLength(11);
        });

        // OrderInformation
        modelBuilder.Entity<OrderInformation>(orderBuilder =>
        {
            orderBuilder.ToTable(nameof(OrderInformation));

            orderBuilder.HasKey(o => o.Id);

            orderBuilder.HasOne(o => o.Owner).WithMany(a => a.OwnershipOrder).HasForeignKey(o => o.OwnerId).OnDelete(DeleteBehavior.ClientSetNull);
            orderBuilder.HasOne<Account>().WithMany(a => a.OrderCreated).HasForeignKey(o => o.CreatorId).OnDelete(DeleteBehavior.ClientSetNull);
            orderBuilder.HasOne(o => o.Driver).WithMany(a => a.OrderReceived).HasForeignKey(o => o.DriverId).OnDelete(DeleteBehavior.ClientSetNull);

            orderBuilder.Property(o => o.DriverId).IsRequired(false);
            orderBuilder.Property(o => o.RecipientName).HasMaxLength(200);
            orderBuilder.Property(o => o.RecipientPhoneNumber).HasMaxLength(11);
            orderBuilder.Property(o => o.SenderName).HasMaxLength(200);
            orderBuilder.Property(o => o.SenderPhoneNumber).HasMaxLength(11);
            orderBuilder.Property(o => o.ShippingProvince).HasMaxLength(200);
            orderBuilder.Property(o => o.ShippingDistrict).HasMaxLength(200);
            orderBuilder.Property(o => o.ShippingWard).HasMaxLength(200);
            orderBuilder.Property(o => o.ShippingAddress).HasMaxLength(200);
            orderBuilder.Property(o => o.ExpectedShippingDate).IsRequired(false);
            orderBuilder.Property(o => o.PickupDate).IsRequired(false);
            orderBuilder.Property(o => o.DropoffDate).IsRequired(false);
            orderBuilder.Property(o => o.Note).HasColumnType("text").IsRequired(false);
        });

        // Feedback
        modelBuilder.Entity<Feedback>(feedbackBuilder =>
        {
            feedbackBuilder.ToTable(nameof(Feedback));

            feedbackBuilder.HasKey(f => f.Id);

            feedbackBuilder.HasOne<OrderInformation>().WithMany().HasForeignKey(f => f.OrderId);
            feedbackBuilder.HasOne<Account>().WithMany().HasForeignKey(f => f.CustomerId).OnDelete(DeleteBehavior.ClientSetNull);
            feedbackBuilder.HasOne<Account>().WithMany().HasForeignKey(f => f.DriverId).OnDelete(DeleteBehavior.ClientSetNull);

            feedbackBuilder.Property(f => f.Comment).IsRequired(false);
        });

        // Notification
        modelBuilder.Entity<NotificationLog>(notificationBuilder =>
        {
            notificationBuilder.ToTable(nameof(NotificationLog));

            notificationBuilder.HasKey(n => n.Id);

            notificationBuilder.HasOne<OrderInformation>().WithMany().HasForeignKey(n => n.OrderId);
            notificationBuilder.HasOne(n => n.Receiver).WithMany().HasForeignKey(n => n.ReceiverId).OnDelete(DeleteBehavior.ClientSetNull);
            notificationBuilder.HasOne(n => n.Driver).WithMany().HasForeignKey(n => n.DriverId).OnDelete(DeleteBehavior.ClientSetNull);

            notificationBuilder.Property(n => n.OrderId).IsRequired(false);
            notificationBuilder.Property(n => n.ReceiverId).IsRequired(false);
            notificationBuilder.Property(n => n.DriverId).IsRequired(false);

            notificationBuilder.Ignore(n => n.Content);
        });

        // OrderAudit
        modelBuilder.Entity<OrderAudit>(orderAuditBuilder =>
        {
            orderAuditBuilder.ToTable(nameof(OrderAudit));

            orderAuditBuilder.HasKey(o => o.Id);

            orderAuditBuilder.HasOne<OrderInformation>().WithMany(o => o.OrderAudits).HasForeignKey(o => o.OrderId);

            orderAuditBuilder.Property(o => o.Description).HasColumnType("text").IsRequired(false);
            orderAuditBuilder.Property(o => o.DriverId).IsRequired(false);
        });
    }
}

