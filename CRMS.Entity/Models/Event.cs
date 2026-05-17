using System;

namespace CRMS.Entity.Models
{
    public class Event
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string EventType { get; set; } // Meeting, Task, Reminder
        public string Location { get; set; }
        public int? CustomerId { get; set; }
        public string Status { get; set; } // Planned, Completed, Cancelled
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public virtual User User { get; set; }
        public virtual Customer Customer { get; set; }
    }
}