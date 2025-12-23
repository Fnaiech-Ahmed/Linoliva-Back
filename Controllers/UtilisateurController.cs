using System;
using System.Collections.Generic;
using tech_software_engineer_consultant_int_backend.DTO.UsersDTOs;
using tech_software_engineer_consultant_int_backend.Models;
using tech_software_engineer_consultant_int_backend.Services;

namespace tech_software_engineer_consultant_int_backend.Controllers
{
    public class UtilisateurController
    {
        private IUserService utilisateurService;

        public UtilisateurController(IUserService _utilisateurService)
        {
            utilisateurService = _utilisateurService;
        }

        public async Task<int> AddUtilisateur(UserCreateDTO utilisateur)
        {
            /*UserCreateDTO utilisateur = new UserCreateDTO
            {
                LastName = nom,
                FirstName = prenom,
                //Role = role,
                Mp = ""
            };*/
            /*return await utilisateurService.CreateUserAsync(utilisateur);*/
            return 1;
        }

        public async Task<IEnumerable<UserDto>> GetUtilisateurs()
        {
            return await utilisateurService.GetUsersAsync();
        }

        public async Task<UserDto> GetUtilisateurById(int id)
        {
            return await utilisateurService.GetUserAsync(id);
        }

        public async Task<User> UpdateUtilisateur(int utilisateurId, UserUpdateDTO utilisateur)
        {
            if (utilisateur != null)
            {
                return await utilisateurService.UpdateUserAsync(utilisateurId, utilisateur);
                // Vous pouvez ajouter d'autres actions ou notifications ici si nécessaire.
            }
            else
            {
                // Vous pouvez gérer cette situation d'une manière appropriée pour votre application.
                return new User();
            }
        }

        public async Task<bool> DeleteUtilisateur(int id)
        {
            return await utilisateurService.DeleteUserAsync(id);
        }
    }
}
