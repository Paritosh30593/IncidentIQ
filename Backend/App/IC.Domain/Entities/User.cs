using System;
using System.Collections.Generic;
using IC.Domain.Common;

namespace IC.Domain.Entities
{
    public class User : BaseEntity<Guid>, IHasCreatedAt, IHasUpdatedAt
    {
        public string ExternalId { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public User CreatedByUser { get; set; }

        public DateTime UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public User UpdatedByUser { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = [];
    }
}
