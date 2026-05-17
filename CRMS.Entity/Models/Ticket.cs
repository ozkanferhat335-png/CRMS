using System;

namespace CRMS.Entity.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string TicketCode { get; set; }
        public int CustomerId { get; set; }
        public int? AssignedToUserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; } // Low, Medium, High, Urgent
        public string Status { get; set; } // Open, InProgress, OnHold, Resolved, Closed
        public string Category { get; set; }
        public string Department { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? CreatedByUserId { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual User AssignedToUser { get; set; }
    }
}