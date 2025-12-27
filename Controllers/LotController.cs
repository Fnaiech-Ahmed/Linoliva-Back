using tech_software_engineer_consultant_int_backend.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using tech_software_engineer_consultant_int_backend.Services;
using tech_software_engineer_consultant_int_backend.DTO.LotsDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace tech_software_engineer_consultant_int_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LotController : ControllerBase
    {
        private readonly ILotService _lotService;
        private readonly ITokenAuthenticationService _tokenAuthService;

        public LotController(ILotService lotService, ITokenAuthenticationService tokenAuthService)
        {
            _lotService = lotService;
            _tokenAuthService = tokenAuthService;
        }

        private bool IsUserAuthorized(params string[] requiredPolicies)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            foreach (var policy in requiredPolicies)
            {
                if (_tokenAuthService.IsUserAuthorized(token, policy))
                    return true;
            }
            return false;
        }

        // -------------------- AJOUT LOT --------------------
        [HttpPost("add-Lot")]
        public async Task<ActionResult<(bool, int)>> AddLot([FromBody] LotCreateDTO newLot)
        {
            if (!IsUserAuthorized("LotPolicy", "LotSeniorPolicy", "AdminPolicy"))
                return Unauthorized();

            if (newLot == null)
                return BadRequest("Lot data is null.");

            try
            {
                int idLot = await _lotService.AddLot(newLot);
                return Ok((true, idLot));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // -------------------- ACHAT LOT --------------------
        [HttpPost("achat-lot")]
        public async Task<ActionResult> AchatLot([FromBody] LotCreateDTO dto)
        {
            Console.WriteLine("AchatQuantity Controller");
            if (!IsUserAuthorized("LotPolicy", "LotSeniorPolicy", "AdminPolicy"))
                return Unauthorized();

            System.Diagnostics.Debug.WriteLine("AchatQuantity Controller");
            if (dto == null)
                return BadRequest("Lot data is null.");

            try
            {
                var result = await _lotService.AchatQuantite(dto);

                if (!result.Success)
                    return BadRequest(result.Message);

                return Ok(new
                {
                    Success = true,
                    LotId = result.LotId,
                    Message = result.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        // -------------------- LISTE LOTS --------------------
        [HttpGet("get-List-Lots")]
        public async Task<ActionResult<List<LotDTO>>> GetLots()
        {
            if (!IsUserAuthorized("LotPolicy", "LotSeniorPolicy", "AdminPolicy"))
                return Unauthorized();

            try
            {
                var lots = await _lotService.GetLots();
                return Ok(lots);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // -------------------- GET LOT BY ID --------------------
        [HttpGet("get-Lot-ByID/{id}")]
        public async Task<ActionResult<LotDTO?>> GetLotById(int id)
        {
            if (!IsUserAuthorized("LotPolicy", "LotSeniorPolicy", "AdminPolicy"))
                return Unauthorized();

            try
            {
                var lot = await _lotService.GetLotById(id);
                if (lot == null)
                    return NotFound();

                return Ok(lot);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // -------------------- UPDATE LOT --------------------
        [HttpPut("update-Lot/{lotId}")]
        public async Task<ActionResult<bool>> UpdateLot(int lotId, [FromBody] LotUpdateDTO lot)
        {
            if (!IsUserAuthorized("LotPolicy", "LotSeniorPolicy"))
                return Unauthorized();

            if (lot == null)
                return BadRequest("Lot data is null.");

            try
            {
                bool result = await _lotService.UpdateLot(lotId, lot);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // -------------------- DELETE LOT --------------------
        [HttpDelete("Delete-Lot/{lotId}")]
        public async Task<ActionResult<bool>> DeleteLot(int lotId)
        {
            if (!IsUserAuthorized("LotPolicy", "LotSeniorPolicy"))
                return Unauthorized();

            try
            {
                bool result = await _lotService.DeleteLot(lotId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}