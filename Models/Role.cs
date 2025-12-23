using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tech_software_engineer_consultant_int_backend.Models
{
    // Role hérite de IdentityRole<int>
    public class Role : IdentityRole<int>
    {
        public string Description { get; set; }

        // Relation Many-to-Many avec RolePolicy
        public ICollection<RolePolicyRole> RolePolicyRoles { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
