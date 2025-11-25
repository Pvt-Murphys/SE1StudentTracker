using System.ComponentModel.DataAnnotations;

namespace SE1StudentTracker.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }
        public string Name { get; set; }
    }


    public class StudentProfile
    {
        [Key]
        public string UserId { get; set; }
        public string StudentNumber { get; set; }
        public string ProgramName { get; set; }
        public int? CohortYear { get; set; }

    }

    public class InstructorProfile
    {
        [Key]
        public string UserId { get; set; }
        public string Department { get; set; }
        public string Title { get; set; }

    }

    public class Course
    {
        [Key]
        public int CourseId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string Term { get; set; }
        public decimal? CreditHours { get; set; }
    }

    public class Section
    {
        [Key]
        public int SectionId { get; set; }
        public int CourseId { get; set; }
        public string SectionCode { get; set; }
        public string Term { get; set; }
        public string PrimaryInstructorId { get; set; }

        public Course Course { get; set; }
        public InstructorProfile PrimaryInstructor { get; set; }
    }

    /*public class Enrollment
    {
        [Key]
        public int EnrollmentId { get; set; }
        public int SectionId { get; set; }
        public string StudentId { get; set; }
        public DateTime EnrolledOn { get; set; }
        public string Status { get; set; } = "enrolled";

        public Section Section { get; set; }
        public string Student { get; set; }
    }*/

    public class TimeSession
    {
        [Key]
        public int SessionId { get; set; }
        public string UserId { get; set; }
        public string SessionType { get; set; }
        public string LocationText { get; set; }
        public int? SectionId { get; set; }
        public DateTime ClockInAt { get; set; }
        public DateTime? ClockOutAt { get; set; }
        public int? DurationMinutes { get; set; }
        public string Source { get; set; } = "web";
        public string Status { get; set; } = "open";
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Section Section { get; set; }
    }

    /* public class AuditLog
    {
        [Key]
        public int AuditId { get; set; }
        public int ActorUserId { get; set; }
        public string TargetTable { get; set; }
        public string TargetPk { get; set; }
        public string Action { get; set; }
        public string BeforeJson { get; set; }
        public string AfterJson { get; set; }
        public string Reason { get; set; }
        public DateTime CreatedAt { get; set; }

        public UserAccount ActorUser { get; set; }
    } */

    /*
    public class Permission
    {
        [Key]
        public int PermissionId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }

    public class RolePermission
    {
        [Key]
        public int RoleId { get; set; }
        public int PermissionId { get; set; }

        public Role Role { get; set; }
        public Permission Permission { get; set; }
    }*/
}
