using System;

namespace CRMS.Entity.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public bool IsLocked { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public DateTime LastLoginDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}