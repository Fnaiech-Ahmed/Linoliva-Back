using tech_software_engineer_consultant_int_backend.DTO.MessagesDTOs;
using tech_software_engineer_consultant_int_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public class MessageService : IMessageService
    {
        private readonly MyDbContext _context;

        public MessageService(MyDbContext context)
        {
            _context = context;
        }

        public async Task<List<Message>> GetMessagesAsync()
        {
            return await _context.Messages.ToListAsync();
        }

        public async Task<Message> GetMessageAsync(int id)
        {
            return await _context.Messages.FindAsync(id);
        }

        public async Task<(bool, string)> CreateMessageAsync(int senderId, int receiverGroupId, string messageContent)
        {
            User? sender = await _context.Users.FindAsync(senderId);
            if (sender == null)
            {
                return (false, "Invalid senderId");
            }

            Group? receiverGroup = await _context.Groups.FindAsync(receiverGroupId);
            if (receiverGroup == null)
            {
                return (false, "Invalid receiverGroupId");
            }

            Message message = new Message
            {
                Content = messageContent,
                Sender = sender,
                ReceiverGroup = receiverGroup
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return (true, "Message envoyé avec succès.");

        }

        public async Task<(bool, List<MessageDto>, string)> GetMessagesByGroupId(int groupId, int userIdInt)
        {
            Group? existingGroup = await _context.Groups.FindAsync(groupId);
            User? existingUser = await _context.Users.FindAsync(userIdInt);

            if (existingGroup != null && existingUser != null)
            {
                bool isUserInGroup = await _context.UserGroupAssociations
                    .AnyAsync(uga => uga.GroupId == groupId && uga.UserId == userIdInt);

                if (isUserInGroup)
                {
                    var messages = await _context.Messages
                        .Where(m => m.ReceiverGroupId == groupId)
                        .Select(m => new MessageDto
                        {
                            Id = m.Id,
                            Content = m.Content,
                            Timestamp = m.Timestamp,
                        })
                        .ToListAsync();

                    return (true, messages, "Liste récupérée avec succès.");
                }
                else
                {
                    List<MessageDto> ListeVide = new List<MessageDto>();
                    return (false, ListeVide, "Vous n'êtes pas membre de ce groupe.");
                }
            }
            else if (existingGroup == null && existingUser != null)
            {
                List<MessageDto> nvlListe = new List<MessageDto>();
                return (false, nvlListe, "groupe introuvé.");
            }
            else if (existingUser == null && existingGroup != null)
            {
                List<MessageDto> nvlListe = new List<MessageDto>();
                return (false, nvlListe, "Utilisateur non authentifié(e).");
            }

            List<MessageDto> nvlListeVide = new List<MessageDto>();
            return (false, nvlListeVide, "");
        }

        public async Task<(bool, string)> UpdateMessageAsync(int idMessage, Message message, int idUser)
        {
            Message? existingMessage = await _context.Messages.FindAsync(idMessage);
            if (existingMessage == null)
            {
                return (false, "Message introuvé.");
            }

            User? existingUser = await _context.Users.FindAsync(idUser);
            if (existingUser == null)
            {
                return (false, "Utilisateur introuvé.");
            }

            if (existingMessage.SenderId == idUser)
            {
                existingMessage.Content = message.Content;

                await _context.SaveChangesAsync();

                return (true, "message mis à jour.");
            }
            else
            {
                return (false, "Erreur de mise à jour.");
            }

        }

        public async Task<(bool, string)> DeleteMessageAsync(int idMessage, int idUser)
        {
            Message? existingMessage = await _context.Messages.FindAsync(idMessage);

            if (existingMessage == null)
            {
                return (false, "Message not found.");
            }

            User? existingUser = await _context.Users.FindAsync(idUser);

            if (existingUser == null)
            {
                return (false, "Utilisateur non trouvé.");
            }

            if (existingMessage.SenderId == existingUser.Id)
            {
                _context.Messages.Remove(existingMessage);
                await _context.SaveChangesAsync();
                return (true, "Message supprimé avec succès.");
            }
            else
            {
                return (false, "Suppression refusée. Vous n'êtes pas l'auteur de ce message.");
            }

        }

    }
}
