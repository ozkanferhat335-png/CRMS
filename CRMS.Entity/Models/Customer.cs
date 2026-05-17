using System;
using System.Collections.Generic;

namespace CRMS.Entity.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string CustomerCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string TaxNumber { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Website { get; set; }
        public string CustomerType { get; set; } // Individual, Corporate
        public string Status { get; set; } // Active, Inactive, Prospect
        public decimal Rating { get; set; }
        public string Notes { get; set; }
        public DateTime? LastContactDate { get; set; }
        public DateTime? NextContactDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? CreatedByUserId { get; set; }
        public int? ModifiedByUserId { get; set; }

        public virtual ICollection<Meeting> Meetings { get; set; }
        public virtual ICollection<Offer> Offers { get; set; }
        public virtual ICollection<Sale> Sales { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}