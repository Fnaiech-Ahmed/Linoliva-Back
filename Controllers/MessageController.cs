using tech_software_engineer_consultant_int_backend.Models;
using tech_software_engineer_consultant_int_backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace tech_software_engineer_consultant_int_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {

        private readonly IMessageService messageService;
        private readonly ITokenAuthenticationService _tokenAuthenticationService;

        public MessageController(IMessageService message, ITokenAuthenticationService tokenAuthenticationService)
        {
            messageService = message;
            _tokenAuthenticationService = tokenAuthenticationService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Message>>> GetMessages()
        {
            var messages = await messageService.GetMessagesAsync();
            return Ok(messages);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Message>> GetMessage(int id)
        {
            var message = await messageService.GetMessageAsync(id);

            if (message == null)
            {
                return NotFound();
            }

            return Ok(message);
        }

        [HttpPost("Create-Message")]
        public async Task<ActionResult<int>> CreateMessage(int senderId, int receiverGroupId, string messageContent)
        {
            var id = await messageService.CreateMessageAsync(senderId, receiverGroupId, messageContent);
            return Ok(id);
        }

        [HttpPut("/Update-Message/{id}")]
        public async Task<ActionResult> UpdateMessage(int id, Message message)
        {
            // Récupérer le jeton d'authentification de l'en-tête de la requête
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            // Valider le jeton en utilisant le service d'authentification des tokens
            bool isValidToken = _tokenAuthenticationService.ValidateToken(token);

            if (isValidToken)
            {
                var userId = _tokenAuthenticationService.GetUserIdFromToken(HttpContext);
                if (!string.IsNullOrEmpty(userId) && int.TryParse(userId, out int userIdInt))
                {
                    try
                    {
                        (bool, string) result = await messageService.UpdateMessageAsync(id, message, userIdInt);
                        return Ok(result.Item2);
                    }
                    catch (ArgumentException ex)
                    {
                        return NotFound(ex.Message);
                    }
                }
                else
                {
                    return BadRequest("Veuillez vous reconnecter s'il vous plaît.");
                }
            }
            else
            {
                // Retourner une réponse d'erreur d'authentification
                return BadRequest("Vous êtes déconnecté. Veuillez vous authentifier s'il vous plaît.");
            }
        }

        [HttpDelete("/Delete-Message/{idMessage}")]
        public async Task<ActionResult> DeleteMessage(int idMessage)
        {
            // Récupérer le jeton d'authentification de l'en-tête de la requête
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            // Valider le jeton en utilisant le service d'authentification des tokens
            bool isValidToken = _tokenAuthenticationService.ValidateToken(token);

            if (isValidToken)
            {
                var userId = _tokenAuthenticationService.GetUserIdFromToken(HttpContext);
                if (!string.IsNullOrEmpty(userId) && int.TryParse(userId, out int userIdInt))
                {
                    try
                    {
                        (bool, string) result = await messageService.DeleteMessageAsync(idMessage, userIdInt);
                        return Ok(result.Item2);
                    }
                    catch (ArgumentException ex)
                    {
                        return NotFound(ex.Message);
                    }
                }
                else
                {
                    return BadRequest("Veuillez vous reconnecter s'il vous plaît.");
                }
            }
            else
            {
                // Retourner une réponse d'erreur d'authentification
                return BadRequest("Vous êtes déconnecté. Veuillez vous authentifier s'il vous plaît.");
            }
        }
    }
}
