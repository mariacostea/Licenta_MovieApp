using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Infrastructure.EntityConfigurations
{
    public class UserMovieConfiguration : IEntityTypeConfiguration<UserMovie>
    {
        public void Configure(EntityTypeBuilder<UserMovie> builder)
        {
            builder.HasKey(um => um.Id);

            builder.Property(um => um.Id)
                .IsRequired();

            builder.Property(um => um.IsWatched)
                .HasDefaultValue(false)
                .IsRequired();
            
        }
    }
}