using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MobyLabWebProgramming.Core.Entities;
using MobyLabWebProgramming.Core.Enums;

namespace MobyLabWebProgramming.Infrastructure.EntityConfigurations;

public sealed class CrewConfiguration : IEntityTypeConfiguration<Crew>
{
    public void Configure(EntityTypeBuilder<Crew> builder)
    {
        builder.ToTable("Crew");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.LastName)
            .IsRequired()
            .HasMaxLength(100);

        // ImageUrl poate fi NULL (problema care ți-a dat eroare)
        builder.Property(c => c.ImageUrl)
            .HasMaxLength(400)
            .IsRequired(false);

        builder.Property(c => c.Birthday)
            .IsRequired(false);

        builder.Property(c => c.TmdbId)
            .IsRequired(false);

        // Index unic pe TmdbId când nu e NULL
        builder.HasIndex(c => c.TmdbId)
            .IsUnique()
            .HasFilter("\"TmdbId\" IS NOT NULL");
    }
}