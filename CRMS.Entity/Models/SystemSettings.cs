using System;

namespace CRMS.Entity.Models
{
    public class SystemSettings
    {
        public int Id { get; set; }
        public string SettingKey { get; set; }
        public string SettingValue { get; set; }
        public string Description { get; set; }
        public string SettingType { get; set; } // String, Int, Boolean, Double
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedByUserId { get; set; }
    }
}