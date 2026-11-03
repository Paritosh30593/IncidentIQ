using System;
using System.Collections.Generic;
using IC.Domain.Common;

namespace IC.Domain.Entities
{
    public class Feature : BaseEntity<int>, IHasCreatedAt, IHasUpdatedAt
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsEnabled { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<Permission> Permissions { get; set; } = [];
    }
}
