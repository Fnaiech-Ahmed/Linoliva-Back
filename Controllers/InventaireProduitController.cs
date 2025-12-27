using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using tech_software_engineer_consultant_int_backend.DTO.InventaireProduitDTOs;
using tech_software_engineer_consultant_int_backend.Services;

namespace tech_software_engineer_consultant_int_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventaireProduitController : ControllerBase
    {
        private readonly IInventaireProduitService _inventaireProduitService;
        private readonly ITokenAuthenticationService _tokenAuthService;

        public InventaireProduitController(IInventaireProduitService inventaireProduitService, ITokenAuthenticationService tokenAuthService)
        {
            _inventaireProduitService = inventaireProduitService;
            _tokenAuthService = tokenAuthService;
        }

        private bool IsUserAuthorized(string token, string requiredPolicy)
        {
            // Validate the token
            bool isValidToken = _tokenAuthService.ValidateToken(token);
            if (!isValidToken) return false;

            // Retrieve user identity from the token
            ClaimsPrincipal user = _tokenAuthService.GetPrincipalFromToken(token);

            // Check if user has the required policy
            var userPolicies = user.FindAll("policy").Select(p => p.Value);
            return userPolicies.Contains(requiredPolicy);
        }

        [HttpPost("Create-InventaireProduit")]
        public async Task<IActionResult> AddInventaireProduit(int productId, string productName, int quantity)
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!IsUserAuthorized(token, "InventaireProduitSeniorPolicy"))
            {
                return Unauthorized("Vous n'êtes pas autorisé à ajouter des produits à l'inventaire.");
            }

            var inventaireProduit = new InventaireProduitCreateDTO
            {
                ProductId = productId,
                ProductName = productName,
                Quantity = quantity
            };
            bool result = await _inventaireProduitService.AddInventaireProduit(inventaireProduit);
            return result ? Ok() : BadRequest("Erreur lors de l'ajout du produit.");
        }

        [HttpGet("Get-List-InventairesProduits")]
        public async Task<ActionResult<List<InventaireProduitDTO>>> GetInventaireProduits()
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!IsUserAuthorized(token, "InventaireProduitPolicy"))
                //InventaireProduitViewPolicy"))
            {
                return Unauthorized("Vous n'êtes pas autorisé à consulter la liste des produits.");
            }

            var inventaireProduits = await _inventaireProduitService.GetInventaireProduits();
            return Ok(inventaireProduits);
        }

        [HttpGet("Get-InventaireProduit/{id}")]
        public async Task<ActionResult<InventaireProduitDTO>> GetInventaireProduitById(int id)
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!IsUserAuthorized(token, "InventaireProduitPolicy"))
            //InventaireProduitViewPolicy"))
            {
                return Unauthorized("Vous n'êtes pas autorisé à consulter ce produit.");
            }

            var inventaireProduit = await _inventaireProduitService.GetInventaireProduitById(id);
            return inventaireProduit != null ? Ok(inventaireProduit) : NotFound("Produit non trouvé.");
        }

        [HttpGet("Verif-InventaireProduit/{ProductId}")]
        public async Task<ActionResult<(bool, int)>> VerifInventaireProduitByProductId(int ProductId)
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!IsUserAuthorized(token, "InventaireProduitPolicy"))
            //InventaireProduitViewPolicy"))
            {
                return Unauthorized("Vous n'êtes pas autorisé à vérifier ce produit.");
            }

            var result = await _inventaireProduitService.VerifInventaireProduitByProductId(ProductId);
            return Ok(result);
        }

        [HttpPut("Update-InventaireProduit/{inventaireProduitId}")]
        public async Task<IActionResult> UpdateInventaireProduit(int inventaireProduitId, [FromBody] InventaireProduitUpdateDTO inventaireProduit)
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!IsUserAuthorized(token, "InventaireProduitAdminPolicy"))
            {
                return Unauthorized("Vous n'êtes pas autorisé à mettre à jour les produits de l'inventaire.");
            }

            if (inventaireProduit != null)
            {
                bool result = await _inventaireProduitService.UpdateInventaireProduit(inventaireProduitId, inventaireProduit);
                return result ? NoContent() : BadRequest("Erreur lors de la mise à jour du produit.");
            }
            else
            {
                return BadRequest("Données de produit invalides.");
            }
        }

        [HttpDelete("Delete-InventaireProduit/{id}")]
        public async Task<IActionResult> DeleteInventaireProduit(int id)
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!IsUserAuthorized(token, "InventaireProduitSeniorPolicy"))
            {
                return Unauthorized("Vous n'êtes pas autorisé à supprimer des produits de l'inventaire.");
            }

            bool result = await _inventaireProduitService.DeleteInventaireProduit(id);
            return result ? NoContent() : BadRequest("Erreur lors de la suppression du produit.");
        }
    }
}
