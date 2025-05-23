using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MobyLabWebProgramming.Core.Entities;
using MobyLabWebProgramming.Core.Enums;

namespace MobyLabWebProgramming.Infrastructure.EntityConfigurations
{
    /// <summary>
    /// Configuration for the User entity.
    /// </summary>
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);
            
            builder.Property(e => e.Id)
                .IsRequired();

            builder.Property(e => e.Name)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(e => e.Email)
                .HasMaxLength(255)
                .IsRequired();
            
            builder.HasAlternateKey(e => e.Email);

            builder.Property(e => e.Password)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(e => e.Role)
                .HasConversion(new EnumToStringConverter<UserRoleEnum>())
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(e => e.CreatedAt)
                .IsRequired();

            builder.Property(e => e.UpdatedAt)
                .IsRequired();
            
            builder.Property(e => e.EmailConfirmed)
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(e => e.EmailConfirmationToken)
                .HasMaxLength(255)
                .IsRequired(false);
        }
    }
}