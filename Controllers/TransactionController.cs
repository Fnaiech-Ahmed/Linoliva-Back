using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Transactions;
using System.Windows.Input;

using tech_software_engineer_consultant_int_backend.DTO.TransactionsDTOs;
//using tech_software_engineer_consultant_int_backend.DTOs.ProductsDTOs;
using tech_software_engineer_consultant_int_backend.Models;
using tech_software_engineer_consultant_int_backend.Services;

namespace tech_software_engineer_consultant_int_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private ITransactionService _ITransactionService;
        private readonly ITokenAuthenticationService _tokenAuthService;

        public TransactionController(ITransactionService iTransactionService, ITokenAuthenticationService tokenAuthService)
        {
            _ITransactionService = iTransactionService;
            _tokenAuthService = tokenAuthService;
        }



        [HttpPost("addTransaction")]
        public async Task<(bool, int)> AddTransaction([FromBody] TransactionCreateDto transactionCreateDto, string ReferenceCommande)
        {
            return await _ITransactionService.AddTransaction(transactionCreateDto, ReferenceCommande);
        }


        [HttpGet("GetTransactions")]
        public async Task<ActionResult<List<TransactionsDTO>>> GetTransactions()
        {
            // Récupérer le jeton d'authentification de l'en-tête de la requête
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            // Valider le jeton
            bool isValidToken = _tokenAuthService.ValidateToken(token);

            if (isValidToken)
            {
                var transactions = await _ITransactionService.GetTransactions();
                return transactions;
            } else
            {
                return BadRequest("Vous êtes déconnecté. Veuillez vous authentifier svp."); // Retourner une réponse d'erreur d'authentification
            }
        }


        //[HttpGet("getTransactionbyRef/{Ref}")]
        /*public async Task<List<TransactionsDTO>> GetTransactionsByRefCommande(string ReferenceCommande)
        {
            return await _ITransactionService.GetTransactionsByRefCommande(ReferenceCommande);
        }*/
        [HttpGet("getTransactionbyRef/{ReferenceCommande}")]
        public async Task<List<TransactionsDTO>> GetTransactionsByRefCommande(string ReferenceCommande)
        {
            return await _ITransactionService.GetTransactionsByRefCommande(ReferenceCommande);
        }


        [HttpGet("getTransactionbyId/{id}")]
        public async Task<TransactionsDTO> GetTransactionById(int id)
        {
            return await _ITransactionService.GetTransactionById(id);
        }


        [HttpPut("updateTransaction/{id}")]
        public async Task<(bool, int)> UpdateTransaction(int TransactionId, [FromBody] TransactionsUpdateDTO Transaction)
        {
            if (Transaction != null)
            {
                return await _ITransactionService.UpdateTransaction(TransactionId, Transaction);
                // Vous pouvez ajouter d'autres actions ou notifications ici si nécessaire.
            }
            else
            {
                // Vous pouvez gérer cette situation d'une manière appropriée pour votre application.
                return (false, 0);
            }
        }


        [HttpDelete("deleteTransactionbyId/{id}")]
        public async Task<bool> DeleteTransaction(int id)
        {
            bool resultatSuppressionTransaction = await _ITransactionService.DeleteTransaction(id);
            return resultatSuppressionTransaction;
        }
    }
}
