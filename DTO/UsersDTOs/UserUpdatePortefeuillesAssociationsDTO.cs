using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.DTO.UsersDTOs
{
    public class UserUpdatePortefeuillesAssociationsDTO
    {
        public List<UserPortefeuilleAssociation>? ListeUserPortefeuilles { get; set; }


        // Constructeur par défaut (vide)
        public UserUpdatePortefeuillesAssociationsDTO()
        {
        }

        // Constructeur de copie depuis une entité User
        public UserUpdatePortefeuillesAssociationsDTO(User user)
        {
            ListeUserPortefeuilles = user.ListeUserPortefeuilles;
        }

        // Constructeur avec paramètres pour les propriétés obligatoires
        public User ToUserEntity()
        {
            return new User
            {
                ListeUserPortefeuilles = ListeUserPortefeuilles,
            };
        }


    }
}
