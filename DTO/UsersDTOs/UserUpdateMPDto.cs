using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.DTO.UsersDTOs
{
    public class UserUpdateMPDto
    {
        public string? MP { get; set; }

        public UserUpdateMPDto() { }

        // Constructeur de copie depuis une entité User
        public UserUpdateMPDto(User user)
        {
            MP = user.Mp;
        }

        // Méthode pour convertir un UserDto en entité User
        public User ToUserEntity()
        {
            return new User
            {
                Mp = MP,
            };
        }

    }
}
