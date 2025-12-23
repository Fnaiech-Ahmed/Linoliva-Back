using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.DTO.UsersDTOs
{
    public class UserListeUPAsDTO
    {
        public int Id;
        public string RefUser;
        public List<UserPortefeuilleAssociation>? ListeUserPortefeuilles { get; set; }


        public UserListeUPAsDTO()
        {
        }

        // Constructeur de copie depuis une entité User
        public UserListeUPAsDTO(User user)
        {
            Id = user.Id;
            RefUser = user.Ref;
            ListeUserPortefeuilles = user.ListeUserPortefeuilles;
        }

        // Méthode pour convertir un UserDto en entité User
        public User ToUserEntity()
        {
            return new User
            {
                Id = Id,
                Ref = RefUser,
                ListeUserPortefeuilles = ListeUserPortefeuilles
            };
        }

    }
}
