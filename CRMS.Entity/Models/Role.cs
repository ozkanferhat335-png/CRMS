using System;
using System.Collections.Generic;

namespace CRMS.Entity.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<Permission> Permissions { get; set; }
    }
}