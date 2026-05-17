using System;
using System.Collections.Generic;

namespace CRMS.Entity.Models
{
    public class Offer
    {
        public int Id { get; set; }
        public string OfferCode { get; set; }
        public int CustomerId { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public string Status { get; set; } // Pending, Approved, Rejected, Expired
        public DateTime OfferDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? CreatedByUserId { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<OfferItem> OfferItems { get; set; }
    }
}