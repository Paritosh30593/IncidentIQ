using System;
using System.Collections.Generic;
using IC.Domain.Common;

namespace IC.Domain.Entities
{
    public class Role : BaseEntity<int>, IHasCreatedAt
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsSystem { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = [];
        public ICollection<RolePermission> RolePermissions { get; set; } = [];
    }
}
