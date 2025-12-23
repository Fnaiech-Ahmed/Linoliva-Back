namespace tech_software_engineer_consultant_int_backend.Models
{
    public class UserMessageAssociation
    {
        public int Id { get; set; }

        public int? UserId { get; set; } // Clé étrangère vers User
        public User? User { get; set; }
        public string? TypeUser { get; set; } // Sender Or Receiver

        public int? MessageId { get; set; } // Clé étrangère vers MessageId
        public Message? Message { get; set; }
    }
}
