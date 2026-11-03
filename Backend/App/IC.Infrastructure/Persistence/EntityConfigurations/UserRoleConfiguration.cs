using IC.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IC.Infrastructure.Persistence.EntityConfigurations
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable(nameof(UserRole), b => b.IsTemporal(t =>
            {
                t.HasPeriodStart("ValidFrom");
                t.HasPeriodEnd("ValidTo");
            }));

            builder.HasKey(x => new { x.UserId, x.RoleId })
                .HasName("PK_UserRole_UserId_RoleId");

            builder.HasOne(x => x.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(x => x.UserId)
                .HasConstraintName("FK_UserRole_UserId_User_Id")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(x => x.RoleId)
                .HasConstraintName("FK_UserRole_RoleId_Role_Id")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.AssignedByUser)
                .WithMany()
                .HasForeignKey(x => x.AssignedBy)
                .HasConstraintName("FK_UserRole_AssignedBy_User_Id")
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
