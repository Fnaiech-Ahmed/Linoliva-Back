using tech_software_engineer_consultant_int_backend.Models;
using System;
using System.Collections.Generic;
using tech_software_engineer_consultant_int_backend.Services;
using Microsoft.AspNetCore.Mvc;
using tech_software_engineer_consultant_int_backend.DTO.LotsDTOs;

namespace tech_software_engineer_consultant_int_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LotController : ControllerBase
    {
        private readonly ILotService lotService;

        public LotController(ILotService _lotService)
        {
            lotService = _lotService;
        }



        [HttpPost("add-Lot")]
        public async Task<(bool,int)> AddLot([FromBody] LotCreateDTO newLot)
        {
            try
            {
                // Vérifiez que le lot n'est pas nul avant de l'ajouter
                if (newLot != null)
                {
                    int idNewLot = await lotService.AddLot(newLot);
                    // Si l'ajout a réussi, retournez true
                    return (true, idNewLot);
                }
                else
                {
                    // Gérez la situation où le lot est nul d'une manière appropriée
                    //MessageBox.Show("Lot null.");
                    return (false, 0);                    
                }
            }
            catch (Exception ex)
            {
                // Si une exception se produit pendant l'ajout, retournez false
                // Vous pouvez également gérer l'exception de manière appropriée ici
                //MessageBox.Show(ex.Message);
                return (false, 0);
            }
        }



        [HttpGet("get-List-Lots")]
        public async Task<List<LotDTO>> GetLots()
        {
            return await lotService.GetLots();
        }



        [HttpGet("get-Lot-ByID/{id}")]
        public async Task<LotDTO?> GetLotById(int id)
        {
            return await lotService.GetLotById(id);
        }


        [HttpPut("update-Lot/{lotId}")]
        public async Task<bool> UpdateLot(int lotId, [FromBody] LotUpdateDTO lot)
        {
            if (lot != null)
            {
                return await lotService.UpdateLot(lotId, lot);
                // Vous pouvez ajouter d'autres actions ou notifications ici si nécessaire.
            }
            else
            {
                // Vous pouvez gérer cette situation d'une manière appropriée pour votre application.
                return false;
            }
        }



        [HttpDelete("Delete-Lot/{lotId}")]
        public async Task<bool> DeleteLot(int lotId)
        {
            return await lotService.DeleteLot(lotId);
        }
    }
}
