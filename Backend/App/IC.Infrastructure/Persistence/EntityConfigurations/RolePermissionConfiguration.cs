using IC.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IC.Infrastructure.Persistence.EntityConfigurations
{
    public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
    {
        public void Configure(EntityTypeBuilder<RolePermission> builder)
        {
            builder.ToTable(nameof(RolePermission));

            builder.HasKey(x => new { x.RoleId, x.PermissionId })
                .HasName("PK_RolePermission_RoleId_PermissionId");

            builder.HasOne(x => x.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(x => x.RoleId)
                .HasConstraintName("FK_RolePermission_RoleId_Role_Id")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(x => x.PermissionId)
                .HasConstraintName("FK_RolePermission_PermissionId_Permission_Id")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
