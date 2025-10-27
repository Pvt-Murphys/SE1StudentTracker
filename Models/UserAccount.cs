namespace SE1StudentTracker.Models
{
    public class UserAccount
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName  { get; set; } = "";
        public string Email     { get; set; } = "";
        public int RoleId       { get; set; }
    }
}
