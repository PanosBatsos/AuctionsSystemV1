using AuctionsSystem.AccountService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuctionsSystem.AccountService.Infrastructure.Persistence.Configurations
{
    internal class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("accounts");
            builder.HasKey(account => account.Id);


            builder.HasIndex(e => e.Email).IsUnique();
            builder.HasIndex(e => e.UserName).IsUnique();
            builder.HasIndex(e => e.PhoneNumber).IsUnique();
            builder.HasIndex(e => e.IdNumber).IsUnique();

            builder.Property(e => e.Id)
                .HasColumnName("id");

            builder.Property(e => e.Email)
                .HasColumnName("email")
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(e => e.UserName)
                .HasColumnName("username")
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.PhoneNumber)
                .HasColumnName("phone_number")
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(e => e.FirstName)
                .HasColumnName("first_name")
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.LastName)
                .HasColumnName("last_name")
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.IdNumber)
                .HasColumnName("id_number")
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(e => e.Role)
                .HasColumnName("role")
                .IsRequired();

            builder.Property(e => e.IsActive)
                .HasColumnName("is_active")
                .IsRequired();

            builder.Property(e => e.TermsAcceptedAt)
                 .HasColumnName("terms_accepted_at")
                 .IsRequired();

            builder.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            builder.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");

            builder.Property(e => e.LastLoginAt)
                .HasColumnName("last_login_at");

            builder.Property(e => e.LastLoginIp)
                .HasColumnName("last_login_ip")
                .HasMaxLength(45);

            builder.OwnsOne(e => e.Security, security =>
            {
                security.Property(p => p.PasswordHash).HasColumnName("password").IsRequired();
                security.Property(p => p.TwoFactorEnabled).HasColumnName("two_factor_enabled");
                security.Property(p => p.AccessFailedCount).HasColumnName("access_failed_count");
                security.Property(p => p.LockoutEnd).HasColumnName("lockout_end");
            });

            builder.OwnsOne(e => e.Verification, verification =>
            {
                verification.Property(p => p.EmailConfirmed).HasColumnName("email_confirmed");
                verification.Property(p => p.PhoneNumberConfirmed).HasColumnName("phone_number_confirmed");
            });
        }
    }
}
