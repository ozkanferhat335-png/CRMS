using System;

namespace CRMS.Entity.Models
{
    public class OfferItem
    {
        public int Id { get; set; }
        public int OfferId { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TaxRate { get; set; }
        public decimal TaxAmount { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual Offer Offer { get; set; }
    }
}