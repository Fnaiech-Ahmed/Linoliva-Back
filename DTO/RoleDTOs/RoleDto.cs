using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.DTO.RoleDTOs
{
    public class RoleDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        public RoleDto()
        {
        }

        // Constructeur de copie depuis une entité User
        public RoleDto(Role role)
        {
            Id = role.Id;
            Name = role.Name;
            Description = role.Description;
        }

        // Méthode pour convertir un RoleDto en entité Role
        public Role ToRoleEntity()
        {
            return new Role
            {
                Id = Id,
                Name = Name,
                Description = Description
            };
        }

    }
}
