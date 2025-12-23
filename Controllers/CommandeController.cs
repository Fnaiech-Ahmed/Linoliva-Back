using tech_software_engineer_consultant_int_backend.Models;
using System;
using System.Collections.Generic;
using tech_software_engineer_consultant_int_backend.Services;
using tech_software_engineer_consultant_int_backend.DTO.CommandesDTOs;
using tech_software_engineer_consultant_int_backend.DTO.TransactionsDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace tech_software_engineer_consultant_int_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommandeController : ControllerBase
    {
        private readonly ICommandeService _commandeService;
        private readonly ITokenAuthenticationService _tokenAuthService;

        public CommandeController(ICommandeService commandeService, ITokenAuthenticationService tokenAuthService)
        {
            _commandeService = commandeService;
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


        [HttpPost("Create-Commande")]
        public async Task<ActionResult> AddCommande([FromBody] CommandeCreateDTO newCommande, [FromQuery] List<TransactionCreateDto> ListTransactions)
        {
            if (!IsUserAuthorized("CommandeSeniorPolicy")) // Remplacez "requiredPolicy" par la politique réelle requise
            {
                return Unauthorized();
            }

            try
            {
                bool resultatAjoutCommande = await _commandeService.AddCommande(newCommande, ListTransactions);

                if (resultatAjoutCommande)
                {
                    return Ok();
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create commande");
                }
            }
            catch (Exception ex)
            {
                // Si une exception se produit pendant l'ajout, retournez une réponse d'erreur
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }




        [HttpGet("Get-List-Commandes")]
        public async Task<ActionResult<List<CommandeDTO>>> GetCommandes()
        {
            if (IsUserAuthorized("CommandePolicy", "CommandeSeniorPolicy")) // Remplacez "requiredPolicy" par la politique réelle requise
            {
                var commandes = await _commandeService.GetCommandes();
                return Ok(commandes);
                
            }
            else if (IsUserAuthorized("UserPolicy"))
            {
                string currentUserId = _tokenAuthService.GetCurrentUserId(); // Méthode pour récupérer l'ID de l'utilisateur courant
                var ListeCommandesUser = await _commandeService.GetListeCommandeByOwner(currentUserId);
                if (ListeCommandesUser == null)
                {
                    return NotFound();
                }
                return Ok(ListeCommandesUser);
            }
            else
            {
                return Unauthorized();
            }           
            
        }




        [HttpGet("get-Commande-ByID/{id}")]
        public async Task<ActionResult<CommandeDTO?>> GetCommandeById(int id)
        {
            if (IsUserAuthorized("CommandePolicy", "CommandeSeniorPolicy")) // Remplacez "requiredPolicy" par la politique réelle requise
            {
                var commande = await _commandeService.GetCommandeById(id);
                if (commande == null)
                {
                    return NotFound();
                }
                return Ok(commande);
                
            }else if (IsUserAuthorized("UserPolicy"))
            {
                string currentUserId = _tokenAuthService.GetCurrentUserId(); // Méthode pour récupérer l'ID de l'utilisateur courant
                var commande = await _commandeService.GetCommandeById(id);

                if (commande == null)
                {
                    return NotFound();

                }else if (commande.ProprietaireId.ToString() == currentUserId )
                {
                    return Ok(commande);
                }
                else
                {
                    return Unauthorized();
                }
                
            }
            else
            {
                return Unauthorized();
            }

            
        }

        [HttpGet("get-Total-Commande-ByID/{id}")]
        public async Task<ActionResult<(decimal, decimal)>> GetTotalCommandeById(int id)
        {
            if (!IsUserAuthorized("CommandePolicy", "CommandeSeniorPolicy")) // Remplacez "requiredPolicy" par la politique réelle requise
            {
                return Unauthorized();
            }

            var total = await _commandeService.GetTotalCommandeById(id);
            return Ok(total);
        }

        [HttpPut("update-Commande/{commandeId}")]
        public async Task<ActionResult> UpdateCommande(int commandeId, [FromBody] CommandeUpdateDTO commande, [FromQuery] List<Transactions> ListTransactions)
        {
            if (!IsUserAuthorized("CommandeSeniorPolicy")) // Remplacez "requiredPolicy" par la politique réelle requise
            {
                return Unauthorized();
            }

            if (commande == null)
            {
                return BadRequest("Commande data is null");
            }

            var result = await _commandeService.UpdateCommande(commandeId, commande, ListTransactions);
            if (result)
            {
                return NoContent();
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update commande");
            }
        }




        [HttpGet("get-Commande-ByRef/{reference}")]
        public async Task<ActionResult<CommandeDTO?>> GetCommandeByRef(string commandeRef)
        {
            if (IsUserAuthorized("CommandePolicy", "CommandeSeniorPolicy"))
            {
                var commande = await _commandeService.GetCommandeByRef(commandeRef);
                if (commande == null)
                {
                    return NotFound();
                }
                return Ok(commande);
            }
            else if (IsUserAuthorized("UserPolicy"))
            {
                string currentUserId = _tokenAuthService.GetCurrentUserId(); // Méthode pour récupérer l'ID de l'utilisateur courant
                var commande = await _commandeService.GetCommandeByRef(commandeRef);

                if (commande == null)
                {
                    return NotFound();

                }
                else if (commande.ProprietaireId.ToString() == currentUserId)
                {
                    return Ok(commande);
                }
                else
                {
                    return Unauthorized();
                }

            }
            else
            {
                return Unauthorized();
            }

        }




        [HttpDelete("delete-Commande/{id}")]
        public async Task<ActionResult<(bool, string)>> DeleteCommande(int id)
        {
            if (!IsUserAuthorized("CommandeSeniorPolicy")) // Remplacez "requiredPolicy" par la politique réelle requise
            {
                return Unauthorized();
            }

            var result = await _commandeService.DeleteCommande(id);
            if (result.Item1)
            {
                return Ok(result);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result.Item2);
            }
        }
    }
}
