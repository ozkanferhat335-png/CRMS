using System;

namespace CRMS.Entity.Models
{
    public class Task
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int AssignedToUserId { get; set; }
        public int? CustomerId { get; set; }
        public string Priority { get; set; } // Low, Medium, High, Urgent
        public string Status { get; set; } // Pending, InProgress, Completed, Cancelled
        public DateTime DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public int? AssignedByUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public virtual User AssignedToUser { get; set; }
        public virtual User AssignedByUser { get; set; }
        public virtual Customer Customer { get; set; }
    }
}