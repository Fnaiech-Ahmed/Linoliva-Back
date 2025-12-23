using tech_software_engineer_consultant_int_backend.Models;
using Microsoft.AspNetCore.Mvc;
using tech_software_engineer_consultant_int_backend.Services;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace tech_software_engineer_consultant_int_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService notificationService;
        private readonly ITokenAuthenticationService _tokenAuthService;

        public NotificationController(INotificationService service, ITokenAuthenticationService tokenAuthService)
        {
            notificationService = service;
            _tokenAuthService = tokenAuthService;
        }

        private bool IsUserAuthorized(params string[] requiredPolicies)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            foreach (var policy in requiredPolicies)
            {
                if (_tokenAuthService.IsUserAuthorized(token, policy))
                {
                    return true;
                }
            }
            return false;
        }

        [HttpGet("GetNotificationsList")]
        public async Task<ActionResult<List<Notification>>> GetNotifications()
        {
            if (!IsUserAuthorized("NotificationSeniorPolicy", "NotificationPolicy"))
            {
                return Unauthorized(new { Error = "Unauthorized", Message = "Vous n'êtes pas autorisé à accéder à cette ressource." });
            }

            var notifications = await notificationService.GetNotificationsAsync();
            return Ok(notifications);
        }

        [HttpGet("GetNotification/{id}")]
        public async Task<ActionResult<Notification>> GetNotification(int id)
        {
            if (!IsUserAuthorized("NotificationSeniorPolicy", "NotificationPolicy"))
            {
                return Unauthorized(new { Error = "Unauthorized", Message = "Vous n'êtes pas autorisé à accéder à cette ressource." });
            }

            var notification = await notificationService.GetNotificationAsync(id);

            if (notification == null)
            {
                return NotFound();
            }

            return Ok(notification);
        }

        [HttpPost("CreateNotification")]
        public async Task<ActionResult<int>> CreateNotification([FromBody] Notification notification)
        {
            if (!IsUserAuthorized("NotificationSeniorPolicy"))
            {
                return Unauthorized(new { Error = "Unauthorized", Message = "Vous n'êtes pas autorisé à créer une notification." });
            }

            var id = await notificationService.CreateNotificationAsync(notification);
            return Ok(id);
        }

        [HttpPut("UpdateNotification/{id}")]
        public async Task<ActionResult> UpdateNotification(int id, [FromBody] Notification notification)
        {
            if (!IsUserAuthorized("NotificationSeniorPolicy"))
            {
                return Unauthorized(new { Error = "Unauthorized", Message = "Vous n'êtes pas autorisé à mettre à jour une notification." });
            }

            try
            {
                await notificationService.UpdateNotificationAsync(id, notification);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("DeleteNotification/{id}")]
        public async Task<ActionResult> DeleteNotification(int id)
        {
            if (!IsUserAuthorized("NotificationSeniorPolicy", "NotificationPolicy"))
            {
                return Unauthorized(new { Error = "Unauthorized", Message = "Vous n'êtes pas autorisé à supprimer une notification." });
            }

            try
            {
                await notificationService.DeleteNotificationAsync(id);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
