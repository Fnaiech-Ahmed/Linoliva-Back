namespace tech_software_engineer_consultant_int_backend.Models
{
    public class UserGroupAssociation
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int GroupId { get; set; }
        public Group Group { get; set; }
    }
}
