using System.Collections.Generic;
using IC.Domain.Common;

namespace IC.Domain.Entities
{
    public class Permission : BaseEntity<int>
    {
        public string Key { get; set; }
        public string Description { get; set; }
        public int FeatureId { get; set; }

        public Feature Feature { get; set; }
        public ICollection<RolePermission> RolePermissions { get; set; } = [];
    }
}
