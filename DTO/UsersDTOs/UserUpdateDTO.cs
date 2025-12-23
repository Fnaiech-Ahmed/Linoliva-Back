using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.DTO.UsersDTOs
{
    public class UserUpdateDTO
    {
        public string? UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }


        public UserUpdateDTO()
        {
        }

        // Constructeur de copie depuis une entité User
        public UserUpdateDTO(User user)
        {
            UserName = user.UserName;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Mobile = user.Mobile;
            Email = user.Email;
        }

        // Méthode pour convertir un UserDto en entité User
        public User ToUserEntity()
        {
            return new User
            {
                UserName = UserName,
                FirstName = FirstName,
                LastName = LastName,
                Mobile = Mobile,
                Email = Email,
            };
        }

    }
}
