using System;

namespace CRMS.Entity.Models
{
    public class Log
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string Action { get; set; }
        public string TableName { get; set; }
        public int? RecordId { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string Description { get; set; }
        public string IpAddress { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual User User { get; set; }
    }
}