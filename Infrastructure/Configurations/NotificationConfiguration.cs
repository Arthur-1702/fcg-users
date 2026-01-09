using Domain.Entities;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notification");

            builder.HasKey(n => n.NotificationId);
            builder.Property(n => n.NotificationId)
                .HasMaxLength(50);

            builder.Property(n => n.UserId).IsRequired().HasMaxLength(20);
            builder.Property(n => n.Title).IsRequired().HasMaxLength(100);
            builder.Property(n => n.Body).IsRequired().HasMaxLength(500);

            builder.Property(n => n.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()")
                .HasConversion(
                    v => v, // Grava no banco normalmente  
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc) // Força Kind como UTC ao ler  
                );
        }
    }
}
