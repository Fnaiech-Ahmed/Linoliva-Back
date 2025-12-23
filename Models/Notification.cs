namespace tech_software_engineer_consultant_int_backend.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Ref { get; set; } = "";
        public string DateNotif { get; set; } = "";
        public string BodyNotif { get; set; } = "";
        public string StatutNotif { get; set; } = "";


        public int? UserId { get; set; }
        public User? User { get; set; }
    }
}
