using System;

namespace CRMS.Entity.Models
{
    public class Meeting
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime MeetingDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string MeetingType { get; set; } // Call, Meeting, Email, WhatsApp
        public string Result { get; set; }
        public string Notes { get; set; }
        public string Status { get; set; } // Planned, Completed, Cancelled
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? CreatedByUserId { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual User User { get; set; }
    }
}