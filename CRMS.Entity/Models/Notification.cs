using System;

namespace CRMS.Entity.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string NotificationType { get; set; } // Task, Meeting, Sale, Ticket, System
        public string Status { get; set; } // Unread, Read
        public int? RelatedEntityId { get; set; }
        public string RelatedEntityType { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ReadDate { get; set; }

        public virtual User User { get; set; }
    }
}