using System;

namespace SE1StudentTracker.Models
{
    public class TimeSession
    {
        public int SessionId { get; set; }
        public int UserId { get; set; }
        public string SessionType { get; set; } = "";   // IN_CLASS, CLINICAL, etc.
        public string? LocationText { get; set; }       // simple label per your design
        public int? SectionId { get; set; }
        public DateTime ClockIn { get; set; }
        public DateTime? ClockOut { get; set; }
    }
}
