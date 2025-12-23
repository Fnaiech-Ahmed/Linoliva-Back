using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.DTO.RoleDTOs
{
    public class RoleUpdateDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }

        public RoleUpdateDto()
        {
        }

        // Constructeur de copie depuis une entité User
        public RoleUpdateDto(Role role)
        {
            Name = role.Name;
            Description = role.Description;
        }

        // Méthode pour convertir un RoleDto en entité Role
        public Role ToRoleEntity()
        {
            return new Role
            {
                Name = Name,
                Description = Description
            };
        }

    }
}
