using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.DTO.MessagesDTOs
{
    public class MessageDto
    {
        public int Id { get; set; }
        public string? Content { get; set; }
        public DateTime Timestamp { get; set; }


        public MessageDto()
        {
        }


        // Méthode pour convertir MessageDto en entité Message
        public Message ToMessageEntity()
        {
            return new Message
            {
                Content = Content,
                Timestamp = Timestamp,
            };
        }

        // Méthode pour convertir entité Message en MessageDto
        public static MessageDto FromMessageEntity(Message message)
        {
            return new MessageDto
            {
                Content = message.Content,
                Timestamp = message.Timestamp,
            };

        }
    }
}
