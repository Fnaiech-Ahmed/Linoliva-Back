using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tech_software_engineer_consultant_int_backend.Models;
using tech_software_engineer_consultant_int_backend.Services;
using System.Threading.Tasks;


namespace tech_software_engineer_consultant_int_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public ContactController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost]
        public async Task<IActionResult> EnvoyerMessage(ContactMessage message)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _emailService.SendEmailAsync(message);
            return Ok(new { Message = "Message envoyé avec succès !" });
        }
    }
}
