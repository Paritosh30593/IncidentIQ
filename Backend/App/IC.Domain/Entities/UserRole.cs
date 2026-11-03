using System;

namespace IC.Domain.Entities
{
    public class UserRole
    {
        public Guid UserId { get; set; }
        public User User { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }

        public Guid? AssignedBy { get; set; }
        public User AssignedByUser { get; set; }

        public DateTime AssignedAt { get; set; }
    }
}
