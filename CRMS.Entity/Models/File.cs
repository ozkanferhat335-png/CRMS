using System;

namespace CRMS.Entity.Models
{
    public class File
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public long FileSize { get; set; }
        public string FilePath { get; set; }
        public string Category { get; set; }
        public int? CustomerId { get; set; }
        public int? UserId { get; set; }
        public string RelatedEntity { get; set; }
        public int? RelatedEntityId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? CreatedByUserId { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual User User { get; set; }
    }
}