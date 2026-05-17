using System;

namespace CRMS.Entity.DTOs
{
    public class CustomerDTO
    {
        public int Id { get; set; }
        public string CustomerCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Status { get; set; }
        public decimal Rating { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}