using tech_software_engineer_consultant_int_backend.DTO.UsersDTOs;
using tech_software_engineer_consultant_int_backend.Models;
using tech_software_engineer_consultant_int_backend.Repositories;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public class UserPortefeuilleAssociationService:IUserPortefeuilleAssociationService
    {
        private readonly IUserService userService;
        private readonly IUserPortefeuilleAssociationRepository<UserPortefeuilleAssociation> _UserPortefeuilleAssociationRepository;


        public UserPortefeuilleAssociationService(
            IUserService _userService,
            IUserPortefeuilleAssociationRepository<UserPortefeuilleAssociation> _userPortefeuilleAssociationRepository
            )
        {
            userService = _userService;
            _UserPortefeuilleAssociationRepository = _userPortefeuilleAssociationRepository;
        }


        public async Task<(bool, string, string, Portefeuille)> AddUserPortefeuilleAssociation(string userReference, Portefeuille existingPortefeuille)
        {
            const string userNotFoundMsg = "Utilisateur inexistant.";
            const string portefeuilleNotFoundMsg = "Portefeuille inexistant.";
            const string assignmentFailedMsg = "L'affectation a échoué. Erreur survenue dans l'enregistrement de l'affectation. Veuillez réessayer s'il vous plaît.";

            UserDto? existingUser = await userService.GetUserByRef(userReference);
            UserListeUPAsDTO? UserListUPAs = await userService.GetUserListPortefeuilleAssociations(userReference);
            //Portefeuille? existingPortefeuille = await portefeuilleService.GetPortefeuilleAsync(portefeuilleId);

            if (existingUser == null)
                return (false, "Impossible de réaliser l'affectation.", userNotFoundMsg, existingPortefeuille);
            if (existingPortefeuille == null)
                return (false, "Impossible de réaliser l'affectation.", portefeuilleNotFoundMsg, existingPortefeuille);

            // Validation des associations utilisateur-portefeuille existantes
            bool hasExistingAssociation = await _UserPortefeuilleAssociationRepository.ValidateExistingUserPortefeuilleAssociation(existingUser.Id, existingPortefeuille.Id);

            if (hasExistingAssociation)
            {
                UserPortefeuilleAssociation? existingUserPortefeuilleAssociation = await _UserPortefeuilleAssociationRepository.FindByIDsAsync(existingUser.Id, existingPortefeuille.Id);

                bool userContainsUPA = ValidateUserContainsUPA(existingUserPortefeuilleAssociation, UserListUPAs.ToUserEntity());
                bool portefeuilleContainsUPA = ValidatePortefeuilleContainsUPA(existingUserPortefeuilleAssociation, existingPortefeuille);

                bool userAssignmentSuccess = false;
                string userAssignmentMessage = "Affectation à la liste des portefeuilles de l'utilisateur échouée. Le portefeuille est déjà existant dans la liste des portefeuilles de l'utilisateur.";
                User? newUser = null;

                if (!userContainsUPA)
                {
                    (bool success, string item2, User? user) userAssignmentResult = await Affect_UserPortefeuilleAssociation_To_User(existingUserPortefeuilleAssociation, existingUser.ToUserEntity());
                    userAssignmentSuccess = userAssignmentResult.success;
                    userAssignmentMessage = userAssignmentResult.item2;
                    newUser = userAssignmentResult.user;
                }

                bool portefeuilleAssignmentSuccess = false;
                string portefeuilleAssignmentMessage = "Affectation à la liste des utilisateurs du portefeuille échouée.";

                if (!portefeuilleContainsUPA)
                {
                    /*(bool success, string message1) portefeuilleAssignmentResult = await Affect_UserPortefeuilleAssociation_To_Portefeuille(existingUserPortefeuilleAssociation, existingPortefeuille);
                    portefeuilleAssignmentSuccess = portefeuilleAssignmentResult.success;
                    portefeuilleAssignmentMessage = portefeuilleAssignmentResult.message1;*/
                    Portefeuille portefeuilleAssignmentResult = Affect_UserPortefeuilleAssociation_To_Portefeuille(existingUserPortefeuilleAssociation, existingPortefeuille);
                    existingPortefeuille.UserPortefeuilleAssociations = portefeuilleAssignmentResult.UserPortefeuilleAssociations;
                }

                if (userAssignmentSuccess || portefeuilleAssignmentSuccess)
                    return (true, "Succès d'affectation: " + userAssignmentMessage + ". " + portefeuilleAssignmentMessage, "Notre système a terminé l'affectation.", existingPortefeuille);
                else
                    return (false, "Message d'erreur approprié: " + userAssignmentMessage + ". " + portefeuilleAssignmentMessage, "Opération terminée.", existingPortefeuille);
            }
            else
            {
                UserPortefeuilleAssociation newUserPortefeuilleAssociation = new()
                {
                    Portefeuille = existingPortefeuille,
                    PortefeuilleId = existingPortefeuille.Id,
                    User = existingUser.ToUserEntity(),
                    UserId = existingUser.Id,
                };

                (bool success, UserPortefeuilleAssociation userPortefeuilleAssociation) = await _UserPortefeuilleAssociationRepository.AddAsync(newUserPortefeuilleAssociation);
                Console.WriteLine(userPortefeuilleAssociation);

                if (!success)
                    return (false, "Affectation à la liste des utilisateurs du portefeuille échouée.", assignmentFailedMsg, existingPortefeuille);

                // Logique d'affectation
                else
                {            
                    (bool success2, string message2, User) resultat2 = await Affect_UserPortefeuilleAssociation_To_User(userPortefeuilleAssociation, existingUser.ToUserEntity());
                    //return (resultat1.success1 && resultat2.success2, resultat1.message1 + ". " + resultat2.message2, "Opération terminée.");*/
                    Portefeuille portefeuille = Affect_UserPortefeuilleAssociation_To_Portefeuille(userPortefeuilleAssociation, existingPortefeuille);
                    return (resultat2.success2, resultat2.message2, "Opération terminée.", portefeuille);
                }
            }
        }




        public bool ValidatePortefeuilleContainsUPA(UserPortefeuilleAssociation userPortefeuilleAssociation, Portefeuille _Portefeuille)
        {
            if (_Portefeuille == null)
                return false;
            else if (_Portefeuille.UserPortefeuilleAssociations == null)
                return false;
            else if (_Portefeuille.UserPortefeuilleAssociations.Contains(userPortefeuilleAssociation))
                return true;
            else
                return false;
        }
        public bool ValidateUserContainsUPA(UserPortefeuilleAssociation userPortefeuilleAssociation, User _User)
        {
            if (userPortefeuilleAssociation == null)
                return false;
            else if (_User == null || _User.ListeUserPortefeuilles == null)
                return false;
            else if (_User.ListeUserPortefeuilles.Contains(userPortefeuilleAssociation))
                return true;
            else
                return false;
        }


        public Portefeuille Affect_UserPortefeuilleAssociation_To_Portefeuille(UserPortefeuilleAssociation userPortefeuilleAssociation, Portefeuille _Portefeuille)
        {
            if (_Portefeuille.UserPortefeuilleAssociations == null)
            {
                List<UserPortefeuilleAssociation> userPortefeuilleAssociations = new List<UserPortefeuilleAssociation>
                {
                    userPortefeuilleAssociation
                };
                _Portefeuille.UserPortefeuilleAssociations = userPortefeuilleAssociations;                
            }
            else
            {
                List<UserPortefeuilleAssociation> userPortefeuilleAssociations = _Portefeuille.UserPortefeuilleAssociations;
                userPortefeuilleAssociations.Add(userPortefeuilleAssociation);
                _Portefeuille.UserPortefeuilleAssociations = userPortefeuilleAssociations;                
            }

            //(bool success, string message) Resultat = await portefeuilleService.UpdatePortefeuilleUsersAsync(_Portefeuille.Id, _Portefeuille);
            //return (Resultat.success, Resultat.message);
            return _Portefeuille;
        }

        public async Task<(bool, string, User)> Affect_UserPortefeuilleAssociation_To_User(UserPortefeuilleAssociation userPortefeuilleAssociation, User ExistingUser)
        {
            if (ExistingUser.ListeUserPortefeuilles == null)
            {
                List<UserPortefeuilleAssociation> userPortefeuilleAssociations = new List<UserPortefeuilleAssociation>
                {
                    userPortefeuilleAssociation
                };

                ExistingUser.ListeUserPortefeuilles = userPortefeuilleAssociations;
            }
            else
            {
                List<UserPortefeuilleAssociation> userPortefeuilleAssociations = ExistingUser.ListeUserPortefeuilles;
                userPortefeuilleAssociations.Add(userPortefeuilleAssociation);
                ExistingUser.ListeUserPortefeuilles = userPortefeuilleAssociations;                
            }

            (bool success, string message) resultat = await userService.UpdateListePortefeuilleAssociationsUserAsync(ExistingUser.Id, new UserUpdatePortefeuillesAssociationsDTO(ExistingUser));
            return (resultat.success, resultat.message, ExistingUser);
        }

    }
}
