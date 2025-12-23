using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
//using TSE_Consultant_INT_Backend.Models.DTO.SIsDTOs;
using tech_software_engineer_consultant_int_backend.DTO.SIsDTOs;
using tech_software_engineer_consultant_int_backend.Services;

namespace tech_software_engineer_consultant_int_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SIController : ControllerBase
    {
        private readonly ISIService _SIService;
        private readonly ITokenAuthenticationService _tokenAuthService;

        public SIController(ISIService siService, ITokenAuthenticationService tokenAuthService)
        {
            _SIService = siService;
            _tokenAuthService = tokenAuthService;
        }

        private bool IsUserAuthorized(params string[] requiredPolicies)
        {
            // Récupérer le jeton d'authentification de l'en-tête de la requête
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

        //Méthode d'ajout d'un produit.
        [HttpPost("add-SI")]
        public async Task<ActionResult<bool>> AddSI(SICreateDTO siCreateDTO)
        {
            if (IsUserAuthorized("ProductSeniorPolicy", "AdminPolicy"))
            {
                try
                {
                    if (siCreateDTO == null)
                    {
                        return BadRequest("Le s.i. est null.");
                    }
                    
                    return await _SIService.AddSI(siCreateDTO);
                }
                catch(Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ex);
                }
            }
            else
            {
                return Unauthorized("Vous n'êtes pas autorisé à ajouter des produits.");
            }

            
        }

        [HttpGet("get-list-SIs")]
        public async Task<ActionResult<List<SIDTO>>> GetSIs()
        {
            if (IsUserAuthorized("UserPolicy", "ProductPolicy", "ProductSeniorPolicy", "AdminPolicy"))
            {
                try
                {
                    return Ok(await _SIService.GetSIs());
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }                
                
            }
            else
            {
                return Unauthorized("Vous n'êtes pas autorisé à consulter la liste des SIs.");
            }
            
        }

        [HttpGet("externe-get-list-SIs")]       
        public async Task<ActionResult<List<SIDTO>>> ExterneGetSIs()
        {
            try
            {
                return Ok(await _SIService.GetSIs());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }

        [HttpGet("get-SI-by-id/{id}")]
        public async Task<ActionResult<SIDTO>> GetSIById(int id)
        {
            if (IsUserAuthorized("UserPolicy", "ProductPolicy", "ProductSeniorPolicy", "AdminPolicy"))
            {
                try
                {
                    var si = await _SIService.GetSIById(id);
                    if (si == null)
                    {
                        return NotFound();
                    }
                    return Ok(si);
                }
                catch(Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
                
            }
            else
            {
                return Unauthorized("Vous n'êtes pas autorisé à consulter ce SI.");
            }

            
        }

        

        [HttpPut("update-SI/{SIId}")]
        public async Task<ActionResult<bool>> UpdateSI(int SIId, SIUpdateDTO si)
        {
            if (IsUserAuthorized("ProductSeniorPolicy", "AdminPolicy"))
            {
                try
                {
                    if (si == null)
                    {
                        return BadRequest("Le SI est null. Impossible de mettre à jour.");
                    }
                    
                    return await _SIService.UpdateSI(SIId, si);
                }
                catch(Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
            else
            {
                return Unauthorized("Vous n'êtes pas autorisé à mettre à jour ce produit.");
            }

            
        }

        [HttpDelete("delete-SI/{id}")]
        public async Task<ActionResult<bool>> DeleteSI(int id)
        {
            if (IsUserAuthorized("ProductSeniorPolicy", "AdminPolicy"))
            {
                try
                {
                    return await _SIService.DeleteSI(id);
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
            else
            {
                return Unauthorized("Vous n'êtes pas autorisé à supprimer ce SI.");
            }
            
        }
    }
}
