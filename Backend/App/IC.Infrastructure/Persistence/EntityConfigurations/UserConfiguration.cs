using IC.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IC.Infrastructure.Persistence.EntityConfigurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable(nameof(User), b => b.IsTemporal(t =>
            {
                t.HasPeriodStart("ValidFrom");
                t.HasPeriodEnd("ValidTo");
            }));

            builder.HasKey(x => x.Id)
                .HasName("PK_User_Id");

            builder.Property(x => x.ExternalId)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(x => x.DisplayName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.IsActive)
                .HasDefaultValue(true);

            builder.HasIndex(x => x.ExternalId)
                .IsUnique()
                .HasDatabaseName("UK_User_ExternalId");

            builder.HasIndex(x => x.Email)
                .IsUnique()
                .HasDatabaseName("UK_User_Email");

            builder.HasOne(x => x.CreatedByUser)
                .WithMany()
                .HasForeignKey(x => x.CreatedBy)
                .HasConstraintName("FK_User_CreatedBy_User_Id")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.UpdatedByUser)
                .WithMany()
                .HasForeignKey(x => x.UpdatedBy)
                .HasConstraintName("FK_User_UpdatedBy_User_Id")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
