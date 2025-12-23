using tech_software_engineer_consultant_int_backend.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using tech_software_engineer_consultant_int_backend.Services;
using tech_software_engineer_consultant_int_backend.DTO.BLDTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace tech_software_engineer_consultant_int_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BonDeLivraisonController : ControllerBase
    {
        private readonly IBLService _blService;
        private readonly ITokenAuthenticationService _tokenAuthService;

        public BonDeLivraisonController(IBLService blService, ITokenAuthenticationService tokenAuthService)
        {
            _blService = blService;
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

        [HttpPost("Create-BonDeLivraison")]
        public async Task<ActionResult<bool>> AddBonDeLivraison(BLCreateDTO bonDeLivraisonCreateDTO)
        {
            if (!IsUserAuthorized("BLPolicy", "BLSeniorPolicy", "AdminPolicy"))
            {
                return Unauthorized();
            }

            try
            {
                bool resultAjout = await _blService.AddBonDeLivraison(bonDeLivraisonCreateDTO);
                return resultAjout;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("Get-List-BonDeLivraisons")]
        public async Task<ActionResult<List<BLDTO>>> GetBonDeLivraisons()
        {
            if (IsUserAuthorized("BLPolicy", "BLSeniorPolicy", "AdminPolicy"))
            {
                var listeBonsDeLivraisons = await _blService.GetBonDeLivraisons();
                return Ok(listeBonsDeLivraisons);
                
            }else if (IsUserAuthorized("UserPolicy"))
            {
                string currentUserId = _tokenAuthService.GetCurrentUserId(); // Méthode pour récupérer l'ID de l'utilisateur courant
                var ListebonDeLivraisonUser = await _blService.GetListeBonsDeLivraisonByOwner(currentUserId);
                if (ListebonDeLivraisonUser == null)
                {
                    return NotFound();
                }
                return Ok(ListebonDeLivraisonUser);
            }
            else
            {
                return Unauthorized();
            }
            
        }

        [HttpGet("get-Bon-De-Livraison-ID/{id}")]
        public async Task<ActionResult<BLDTO?>> GetBonDeLivraisonById(int id)
        {
            if (IsUserAuthorized("BLPolicy", "BLSeniorPolicy", "AdminPolicy"))
            {
                var bonDeLivraison = await _blService.GetBonDeLivraisonById(id);
                if (bonDeLivraison == null)
                {
                    return NotFound();
                }
                return Ok(bonDeLivraison);
            }
            else if (IsUserAuthorized("UserPolicy"))
            {
                string currentUserId = _tokenAuthService.GetCurrentUserId(); // Méthode pour récupérer l'ID de l'utilisateur courant
                var bonDeLivraisonUser = await _blService.GetBonDeLivraisonByIdAndOwner(id, currentUserId);
                if (bonDeLivraisonUser == null)
                {
                    return NotFound();
                }
                return Ok(bonDeLivraisonUser);
            }
            else
            {
                return Unauthorized();
            }            
        }

        [HttpGet("get-Bon-De-Livraison-Ref/{RefBL}")]
        public async Task<ActionResult<BLDTO?>> GetBonDeLivraisonByRef(string RefBL)
        {
            if (IsUserAuthorized("BLPolicy", "BLSeniorPolicy", "AdminPolicy"))
            {
                var bonDeLivraison = await _blService.GetBonDeLivraisonByReference(RefBL);
                if (bonDeLivraison == null)
                {
                    return NotFound();
                }
                return Ok(bonDeLivraison);
            }
            else if (IsUserAuthorized("UserPolicy"))
            {
                string currentUserId = _tokenAuthService.GetCurrentUserId(); // Méthode pour récupérer l'ID de l'utilisateur courant
                var bonDeLivraisonUser = await _blService.GetBonDeLivraisonByReferenceAndOwner(RefBL, currentUserId);
                if (bonDeLivraisonUser == null)
                {
                    return NotFound();
                }
                return Ok(bonDeLivraisonUser);
            }
            else
            {
                return Unauthorized();
            }           
            
        }





        [HttpPut("update-BonDeLivraison/{id}")]
        public async Task<ActionResult<bool>> UpdateBonDeLivraison(int bonDeLivraisonId, BLUpdateDTO bonDeLivraisonUpdateDTO)
        {
            if (!IsUserAuthorized("BLPolicy", "BLSeniorPolicy"))
            {
                return Unauthorized();
            }

            if (bonDeLivraisonUpdateDTO != null)
            {
                try
                {
                    bool resultat = await _blService.UpdateBonDeLivraison(bonDeLivraisonId, bonDeLivraisonUpdateDTO);
                    return resultat;
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }
            }
            else
            {
                return BadRequest("Bon de livraison data is null");
            }
        }





        [HttpDelete("delete-BonDeLivraison/{id}")]
        public async Task<ActionResult<bool>> DeleteBonDeLivraison(int id)
        {
            if (!IsUserAuthorized("BLPolicy", "BLSeniorPolicy"))
            {
                return Unauthorized();
            }

            var result = await _blService.DeleteBonDeLivraison(id);
            return Ok(result);
        }
    }
}
