using System;

namespace SE1StudentTracker.Models
{
    public class TimeSessionFull
    {
        public int SessionId { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName  { get; set; } = "";
        public string Email     { get; set; } = "";
        public int RoleId       { get; set; }
        public string RoleName  { get; set; } = "";
        public string SessionType { get; set; } = "";
        public string? LocationText { get; set; }
        public int? SectionId { get; set; }
        public string? SectionCode { get; set; }
        public string? CourseCode  { get; set; }
        public string? CourseName  { get; set; }
        public DateTime ClockIn { get; set; }
        public DateTime? ClockOut { get; set; }
    }
}
