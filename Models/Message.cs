namespace tech_software_engineer_consultant_int_backend.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string? Content { get; set; }
        public DateTime Timestamp { get; set; }

        public int SenderId { get; set; }
        public User? Sender { get; set; }
        public int? ReceiverGroupId { get; set; }
        public Group? ReceiverGroup { get; set; }


        public Message()
        {
            Timestamp = DateTime.Now; // Définit la date actuelle lors de la création de l'instance
        }
    }
}
