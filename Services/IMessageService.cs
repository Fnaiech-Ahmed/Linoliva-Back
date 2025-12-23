using tech_software_engineer_consultant_int_backend.DTO.MessagesDTOs;
using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public interface IMessageService
    {
        Task<List<Message>> GetMessagesAsync();
        Task<Message> GetMessageAsync(int id);
        Task<(bool, string)> CreateMessageAsync(int senderId, int receiverGroupId, string messageContent);
        Task<(bool, string)> UpdateMessageAsync(int id, Message message, int idUser);
        Task<(bool, List<MessageDto>, string)> GetMessagesByGroupId(int groupId, int userIdInt);
        Task<(bool, string)> DeleteMessageAsync(int idMessage, int idUser);
    }
}
