using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MobyLabWebProgramming.Core.Entities;
using MobyLabWebProgramming.Core.Enums;

namespace MobyLabWebProgramming.Infrastructure.EntityConfigurations;

public sealed class MovieCrewConfiguration : IEntityTypeConfiguration<MovieCrew>
{
    public void Configure(EntityTypeBuilder<MovieCrew> builder)
    {
        builder.ToTable("MovieCrew");

        builder.HasKey(mc => mc.Id);

        builder.Property(mc => mc.PersonType)
            .HasConversion<int>()
            .IsRequired();
        
        builder.HasOne(mc => mc.Movie)
            .WithMany(m => m.MovieCrews)
            .HasForeignKey(mc => mc.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(mc => mc.Crew)
            .WithMany(c => c.MovieCrews)
            .HasForeignKey(mc => mc.CrewId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(mc => new { mc.MovieId, mc.CrewId })
            .IsUnique();
    }
}