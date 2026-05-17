using System;

namespace CRMS.Entity.DTOs
{
    public class LoginResponseDTO
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public UserDTO User { get; set; }
        public string Token { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}