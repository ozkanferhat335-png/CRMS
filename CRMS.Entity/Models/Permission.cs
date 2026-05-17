using System;

namespace CRMS.Entity.Models
{
    public class Permission
    {
        public int Id { get; set; }
        public string PermissionName { get; set; }
        public string Description { get; set; }
        public string Module { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int RoleId { get; set; }

        public virtual Role Role { get; set; }
    }
}