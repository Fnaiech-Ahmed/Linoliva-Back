using tech_software_engineer_consultant_int_backend.Services;
using Microsoft.AspNetCore.SignalR;

namespace SignalRWebpack.Hubs;

public class ChatHub : Hub
{
    private readonly Dictionary<int, bool> _groupMembers = new Dictionary<int, bool>();
    private readonly IMessageService _messageService;
    private readonly ITokenAuthenticationService _tokenAuthenticationService;
    private readonly IGroupService _groupService;

    public ChatHub(IMessageService messageService, ITokenAuthenticationService tokenAuthenticationService, IGroupService groupService)
    {
        _messageService = messageService;
        _tokenAuthenticationService = tokenAuthenticationService;
        _groupService = groupService;
    }

    /*    [HubMethodName("mycustomonconnected")]
        public async override Task OnConnectedAsync()
        {
            var groupId = Context.GetHttpContext().Request.Query["groupId"].ToString();

            // Vérifiez d'abord si le groupe existe
            bool groupExists = await _groupService.GroupExists(int.Parse(groupId));
            if (!groupExists)
            {
                throw new HubException("Le groupe spécifié n'existe pas.");
            }

            // Récupérer l'ID de l'utilisateur à partir du jeton
            string userId = _tokenAuthenticationService.GetUserIdFromToken(Context.GetHttpContext());

            // Vérifiez que l'ID de l'utilisateur n'est pas vide et est valide
            if (!string.IsNullOrEmpty(userId) && int.TryParse(userId, out int userIdInt))
            {
                // Vérifiez ici si l'utilisateur est membre du groupe
                bool isUserMember = await _groupService.IsUserInGroupByIds(userIdInt, int.Parse(groupId));

                if (isUserMember)
                {
                    // Ajoutez la connexion à un groupe spécifique basé sur l'ID du groupe
                    await Groups.AddToGroupAsync(Context.ConnectionId, groupId);

                    // Enregistrez l'utilisateur comme membre du groupe (s'il n'est pas déjà enregistré)
                    bool isUserAlreadyRegistered = _groupMembers.ContainsKey(userIdInt) && _groupMembers[userIdInt];
                    if (!isUserAlreadyRegistered)
                    {
                        _groupMembers[userIdInt] = true;
                    }

                    (bool, List<MessageDto>, string) resultat = await _messageService.GetMessagesByGroupId(int.Parse(groupId), userIdInt);

                    // Envoyer les messages aux clients du groupe spécifié
                    await Clients.Group(groupId.ToString()).SendAsync("groupMessagesReceived", resultat.Item2);
                }
                else
                {
                    throw new HubException("Utilisateur n'est pas membre du groupe.");
                }
            }
            else
            {
                // Rejeter la connexion si l'ID de l'utilisateur n'est pas valide ou est absent
                throw new HubException("ID utilisateur invalide.");
            }
        }
    */

    public async override Task OnConnectedAsync()
    {
        await Clients.Caller.SendAsync("ReceiveWelcomeMessage", "Welcome to the chat room!"); // Envoie le message de bienvenue au nouveau client
        await base.OnConnectedAsync();
    }

    public async Task NewMessage(string message)
    {       
        await Clients.All.SendAsync("messageReceived", $"{Context.ConnectionId} : {message}");
    }
 


    public async Task GetGroupMessages(int groupId, int userId)
    {
        var groupMessages = await _messageService.GetMessagesByGroupId(groupId, userId);
        await Clients.Caller.SendAsync("groupMessagesReceived", groupMessages);
    }


    public async Task SendMessageToGroup(int groupId, int userId, string messageContent)
    {
        await Clients.Group(groupId.ToString()).SendAsync("ReceiveMessage", userId, messageContent);
    }

}