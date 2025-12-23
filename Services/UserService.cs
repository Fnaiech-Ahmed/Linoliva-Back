using tech_software_engineer_consultant_int_backend.DTO.UsersDTOs;
using tech_software_engineer_consultant_int_backend.Models;
using tech_software_engineer_consultant_int_backend.Repositories;
using System.Text;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public class UserService:IUserService
    {
        private readonly IUserRepository<User> userRepository;
        private readonly ISequenceRepository<Sequence> sequenceRepository;

        public UserService(IUserRepository<User> _userRepository, ISequenceRepository<Sequence> _sequenceRepository)
        {
            userRepository = _userRepository;
            sequenceRepository = _sequenceRepository;
        }

        /*public async Task<IEnumerable<UserDto>> GetUsersAsync()
        {
            //return
            IEnumerable<User> usersList = userRepository.GetAll();
            if (usersList == null)
            {
                return Enumerable.Empty<UserDto>();
            }
            else
            {
                IEnumerable<UserDto> usersDto = new List<UserDto>();
                foreach (var user in usersList)
                {
                    var userDto = new UserDto(user);
                    usersDto.Append(userDto);
                }

                return usersDto;
            }
        }*/

        public async Task<List<UserDto>> GetUsersAsync()
        {
            //return
            List<User> usersList = await userRepository.GetAll();
            if (usersList == null)
            {
                return new List<UserDto>();
            }
            else
            {
                List<UserDto> usersDto = new List<UserDto>();
                foreach (var user in usersList)
                {
                    var userDto = new UserDto(user);
                    usersDto.Add(userDto);
                }

                return usersDto;
            }
        }

        public async Task<UserDto> GetUserAsync(int id)
        {
            User? existingUser = await userRepository.GetById(id);

            if (existingUser == null) {
                throw new ArgumentException("User not found.");
            }

            var existingUserDto = new UserDto(existingUser);

            return existingUserDto;
        }


        public async Task<UserListeUPAsDTO?> GetUserListPortefeuilleAssociations(string UserRef)
        {
            User? existingUser = await userRepository.FindUserByReference(UserRef);

            if (existingUser == null)
            {
                throw new ArgumentException("User not found.");
            }

            var existingUserListeUPAsDto = new UserListeUPAsDTO(existingUser);

            return existingUserListeUPAsDto;
        }

        public async Task<UserDto?> GetUserByRef(string UserRef)
        {
            User? existingUser = await userRepository.FindUserByReference(UserRef);
            return existingUser == null ? null : new UserDto(existingUser);
        }

        /*public async Task<int> CreateUserAsync(UserCreateDTO userCreateDTO)
        {
            // Utiliser la méthode de conversion pour créer une nouvelle entité User à partir du DTO
            User newUser = userCreateDTO.ToUserEntity();

            await userRepository.Add(newUser);

            return newUser.Id;
        }*/
        public async Task<bool> CreateUserAsync(UserCreateDTO userCreateDTO)
        {
            // Utiliser la méthode de conversion pour créer une nouvelle entité User à partir du DTO
            User newUser = userCreateDTO.ToUserEntity();
            newUser.Ref = await this.GenerateNextRef();

            bool resultat = await userRepository.Add(newUser);

            //return newUser.Id;
            return resultat;
        }
        public async Task<User> UpdateUserAsync(int id, UserUpdateDTO userUpdateDTO)
        {
            User? existingUser = await userRepository.GetById(id);

            if (existingUser == null)
            {
                // Gérer le cas où l'utilisateur n'est pas trouvé
                return new User();
            }

            // Utiliser la méthode de conversion pour mettre à jour les propriétés de l'utilisateur
            var updatedUser = userUpdateDTO.ToUserEntity();

            if (userUpdateDTO.UserName != null)
                existingUser.UserName = updatedUser.UserName;
            if (userUpdateDTO.FirstName != null)
                existingUser.FirstName = updatedUser.FirstName;
            if (userUpdateDTO.LastName != null)
                existingUser.LastName = updatedUser.LastName;
            if (userUpdateDTO.Mobile != null)
                existingUser.Mobile = updatedUser.Mobile;
            if (userUpdateDTO.Email != null)
                existingUser.Email = updatedUser.Email;

            return await userRepository.Update(existingUser);
        }


        public async Task<(bool,string)> generatePasswordAsync(int id)
        {
            User? existingUser = await userRepository.GetById(id);

            if (existingUser == null)
            {
                return (false, "Utilisateur inexistant.");
            }
            else
            {
                Random _random = new Random();
                string Characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                StringBuilder result = new StringBuilder(10); //Longueur (Length) de la chaîne "result" est 10

                for (int i = 0; i < 10; i++)
                {
                    int randomIndex = _random.Next(Characters.Length);
                    result.Append(Characters[randomIndex]);
                }

                existingUser.Mp = result.ToString();

                User? updatedUser = await userRepository.Update(existingUser);
                if (updatedUser !=null && updatedUser.Mp == result.ToString())
                {
                    return (true,"Mot de passe a été modifiée avec succès.");
                }
                else
                {
                    return (false, "Un problème est survenu lors de l'enregistrement du mot de passe. Veuillez réessayer s'il vous plaît.");
                }                
            }
        }


        public async Task<(bool success, string message)> UpdatePasswordUserAsync(int id, UserUpdateMPDto userUpdateMPDto, string ancienMP)
        {
            User? existingUser = await userRepository.GetById(id);

            if (existingUser == null)
            {
                // Gérer le cas où l'utilisateur n'est pas trouvé
                return (false, "Cet utilisateur n'existe pas.");
            }

            // Utiliser la méthode de conversion pour mettre à jour les propriétés de l'utilisateur
            var updatedUser = userUpdateMPDto.ToUserEntity();

            if (userUpdateMPDto.MP != null && existingUser.Mp == ancienMP)
                existingUser.Mp = updatedUser.Mp;

            User? user = await userRepository.Update(existingUser);
            if (user!=null && user.Mp == updatedUser.Mp)
            {
                return (true, "Le mot de passe de " + user.Civilite + " " + user.FirstName + " a été changée avec succès.");
            }
            else
            {
                return (false, "Le mot de passe n'a pas été sauvegardée suite à un problème. Veuillez réessayer s'il vous plaît.");
            }
        }


        public async Task<(bool success, string message)> UpdateListePortefeuilleAssociationsUserAsync(int id, UserUpdatePortefeuillesAssociationsDTO userUpdatePortefeuillesAssociationsDTO)
        {
            User? existingUser = await userRepository.GetById(id);

            if (existingUser == null)
            {
                // Gérer le cas où l'utilisateur n'est pas trouvé
                return (false, "Cet utilisateur n'existe pas.");
            }

            // Utiliser la méthode de conversion pour mettre à jour les propriétés de l'utilisateur
            var updatedUser = userUpdatePortefeuillesAssociationsDTO.ToUserEntity();

            if (userUpdatePortefeuillesAssociationsDTO.ListeUserPortefeuilles != null)
                existingUser.ListeUserPortefeuilles = userUpdatePortefeuillesAssociationsDTO.ListeUserPortefeuilles;

            User? user = await userRepository.Update(existingUser);
            if (user != null && user.Mp == updatedUser.Mp)
            {
                return (true, "La liste des portefeuilles de l'utilisateur: " + existingUser.Civilite + " " + existingUser.FirstName + " " + existingUser.LastName + ", a été changée avec succès.");
            }
            else
            {
                return (false, "La liste n'a pas été changée. Veuillez réessayer s'il vous plaît.");
            }
        }


        public async Task<bool> DeleteUserAsync(int id)
        {
            User? user = await userRepository.GetById(id);

            if (user == null)
            {
                //throw new ArgumentException("User not found.");
                return false;
            }

            return await userRepository.Delete(user);
        }


        /*public string GenerateNextRef()
        {
            var sequence = sequenceRepository.GetSequenceByName("User");
            

            if (sequence == null)
            {
                sequence = new Sequence { Name = "User", NextValue = 1 };

                sequenceRepository.AddSequence(sequence);
            }
            else
            {
                sequence.NextValue++;

                sequenceRepository.UpdateSequence(sequence);
            }

            return $"USERTSECINT-{sequence.NextValue:D5}";
        }*/

        public async Task<string> GenerateNextRef()
        {
            var sequence = sequenceRepository.GetSequenceByName("User");


            if (sequence == null)
            {
                sequence = new Sequence { Name = "User", NextValue = 1 };

                await sequenceRepository.AddSequence(sequence);
            }
            else
            {
                sequence.NextValue++;

                await sequenceRepository.UpdateSequence(sequence);
            }

            return $"USERTSECINT-{sequence.NextValue:D5}";
        }

    }
}
