using IC.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IC.Infrastructure.Persistence.EntityConfigurations
{
    public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable(nameof(Permission));

            builder.HasKey(x => x.Id)
                .HasName("PK_Permission_Id");

            builder.Property(x => x.Key)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            builder.HasIndex(x => x.Key)
                .IsUnique()
                .HasDatabaseName("UK_Permission_Key");

            builder.HasOne(x => x.Feature)
                .WithMany(f => f.Permissions)
                .HasForeignKey(x => x.FeatureId)
                .HasConstraintName("FK_Permission_FeatureId_Feature_Id")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
