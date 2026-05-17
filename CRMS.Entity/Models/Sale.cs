using System;

namespace CRMS.Entity.Models
{
    public class Sale
    {
        public int Id { get; set; }
        public string SaleCode { get; set; }
        public int CustomerId { get; set; }
        public int UserId { get; set; }
        public int? OfferId { get; set; }
        public string Status { get; set; } // Prospect, Quote, Negotiation, Won, Lost, Cancelled
        public decimal Amount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public DateTime SaleDate { get; set; }
        public DateTime? ClosingDate { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? CreatedByUserId { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual User User { get; set; }
        public virtual Offer Offer { get; set; }
    }
}