using tech_software_engineer_consultant_int_backend.DTO.MessagesDTOs;
using tech_software_engineer_consultant_int_backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalRWebpack.Hubs;
using System.Security.Claims;

namespace tech_software_engineer_consultant_int_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatHubController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IMessageService _messageService;
        private readonly IGroupService _groupService;
        private readonly ITokenAuthenticationService _tokenAuthenticationService;

        public ChatHubController(IHubContext<ChatHub> hubContext, IMessageService messageService, IGroupService groupService, ITokenAuthenticationService tokenAuthenticationService)
        {
            _hubContext = hubContext;
            _messageService = messageService;
            _groupService = groupService;
            _tokenAuthenticationService = tokenAuthenticationService;
        }


        [HttpPost("Add-Group")]
        public async Task<ActionResult> AddGroup(string groupName)
        {
            // Récupérer le jeton d'authentification de l'en-tête de la requête
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            // Valider le jeton en utilisant le service d'authentification des tokens
            bool isValidToken = _tokenAuthenticationService.ValidateToken(token);

            if (isValidToken)
            {
                string connectionId = HttpContext.Connection.Id;
                var result = await _groupService.AddGroupAsync(connectionId, groupName);

                if (result.Item1) // Indicateur de réussite (true/false)
                {
                    return Ok(result.Item2); // Résultat réussi
                }
                else
                {
                    return BadRequest(result.Item2); // Erreur
                }
            }

            // Retourner une réponse d'erreur d'authentification
            return BadRequest("Vous êtes déconnecté. Veuillez vous authentifier s'il vous plaît.");
        }


        [HttpPost("send-message-to-new-group")]
        public async Task<IActionResult> SendNewGroupMessage(List<int> ListIdsReceivers, string NewGroupName, string messageContent)
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
                    //Ajouter le nouveau groupe dans la BD. Si l'opération a réussi, nous récupérons la valeur true,
                    //sinon false. Dans le cas favorable nous récupérons l'ID du groupe, sinon nous récupérons 0.
                    (bool, string, int) result = await _groupService.AddGroupAsync(userIdInt.ToString(), NewGroupName);

                    if (result.Item1)
                    {
                        //Parcourir la liste des IDs des Receivers.
                        foreach (int id in ListIdsReceivers)
                        {
                            //Ajouter Receiver au groupe
                            bool resultAddUserToGroup = await _groupService.AddUserToGroup(result.Item3, id);

                            //Control sur l'ajout du Receiver au groupe
                            if (resultAddUserToGroup)
                            {
                                continue;
                            }
                            else
                            {
                                return BadRequest("L'utilisateur avec l'Id: " + id + " est introuvable.");
                            }
                        }

                        (bool, string) resultAddMessage = await _messageService.CreateMessageAsync(userIdInt, result.Item3, messageContent);

                        if (resultAddMessage.Item1)
                        {
                            // Envoyer le message au groupe via SignalR en utilisant le hub
                            await _hubContext.Clients.Group(result.Item3.ToString()).SendAsync("SendMessageToGroup", result.Item3, userIdInt, messageContent);

                            return Ok(result.Item2 + ". " + resultAddMessage.Item2);
                        }
                        else
                        {
                            return BadRequest(resultAddMessage.Item2);
                        }

                    }
                    else
                    {
                        return BadRequest(result.Item2);
                    }
                }
                else
                {
                    return BadRequest("Veuillez vous connecter s'il vous plaît.");
                }
            }
            else
            {
                // Retourner une réponse d'erreur d'authentification
                return BadRequest("Vous êtes déconnecté. Veuillez vous authentifier s'il vous plaît.");
            }
        }


        [HttpPost("send-message-to-group")]
        public async Task<IActionResult> SendGroupMessage(int receiverGroupId, string messageContent)
        {
            var userId = _tokenAuthenticationService.GetUserIdFromToken(HttpContext);

            if (!string.IsNullOrEmpty(userId) && int.TryParse(userId, out int userIdInt))
            {
                (bool, string) resultAddMessage = await _messageService.CreateMessageAsync(userIdInt, receiverGroupId, messageContent);

                if (resultAddMessage.Item1)
                {
                    // Envoyer le message au groupe via SignalR en utilisant le hub
                    await _hubContext.Clients.Group(receiverGroupId.ToString()).SendAsync("SendMessageToGroup", receiverGroupId, userIdInt, messageContent);

                    return Ok(resultAddMessage.Item2);
                }
                else
                {
                    return BadRequest(resultAddMessage.Item2);
                }

            }
            else
            {
                return BadRequest("Veuillez vous connecter s'il vous plaît.");
            }
        }

        [HttpPost("join-group")]
        public async Task<IActionResult> JoinGroup(int groupId)
        {
            // Récupérer l'ID de l'utilisateur à partir du jeton
            var userId = _tokenAuthenticationService.GetUserIdFromToken(HttpContext);

            if (!string.IsNullOrEmpty(userId) && int.TryParse(userId, out int userIdInt))
            {
                bool resultat1Ajout = await _groupService.AddUserToGroup(groupId, userIdInt);
                if (resultat1Ajout)
                {
                    // Rejoindre le groupe SignalR
                    await _hubContext.Groups.AddToGroupAsync(HttpContext.Connection.Id, groupId.ToString());
                    return Ok();
                }
                else
                {
                    return BadRequest("L'utilisateur n'a pas été ajouté suite à une erreur. Merci de réessayer.");
                }
            }
            else
            {
                return BadRequest("L'utilisateur n'a pas été reconnu.");
            }
        }


        [HttpPost("leave-group")]
        public async Task<IActionResult> LeaveGroup(int groupId)
        {
            // Retirer l'utilisateur actuel du groupe
            var userId = GetUserIdFromContext(User);
            await _groupService.RemoveUserFromGroup(groupId, userId);

            // Quitter le groupe SignalR
            await _hubContext.Groups.RemoveFromGroupAsync(HttpContext.Connection.Id, groupId.ToString());

            return Ok();
        }



        // Méthode utilitaire pour obtenir l'ID de l'utilisateur à partir du contexte
        private int GetUserIdFromContext(ClaimsPrincipal user)
        {
            if (user == null)
            {
                return 0;
            }
            else
            {
                // Ajoutez la logique pour récupérer l'ID de l'utilisateur à partir du contexte
                return int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));
            }

        }


        [HttpGet("get-messages")]
        public async Task<IActionResult> GetMessages(int groupId)
        {
            // Récupérer l'ID de l'utilisateur à partir du jeton
            var userId = _tokenAuthenticationService.GetUserIdFromToken(HttpContext);


            if (!string.IsNullOrEmpty(userId) && int.TryParse(userId, out int userIdInt))
            {
                // Récupérer les messages du groupe à partir du service de messages
                (bool, List<MessageDto>, string) result = await _messageService.GetMessagesByGroupId(groupId, userIdInt);

                if (result.Item1)
                {
                    // Envoyer les messages au client via SignalR
                    await _hubContext.Clients.Client(userId).SendAsync("ReceiveMessages", result.Item2);

                    return Ok(result.Item2);
                }
                else
                {
                    return BadRequest(result.Item3);
                }

            }
            else
            {
                return BadRequest("Veuillez vous connecter s'il vous plaît.");
            }

        }


    }
}
