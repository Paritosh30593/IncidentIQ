using IC.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IC.Infrastructure.Persistence.EntityConfigurations
{
    public class FeatureConfiguration : IEntityTypeConfiguration<Feature>
    {
        public void Configure(EntityTypeBuilder<Feature> builder)
        {
            builder.ToTable(nameof(Feature));

            builder.HasKey(x => x.Id)
                .HasName("PK_Feature_Id");

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            builder.Property(x => x.IsEnabled)
                .HasDefaultValue(true);

            builder.HasIndex(x => x.Name)
                .IsUnique()
                .HasDatabaseName("UK_Feature_Name");
        }
    }
}
