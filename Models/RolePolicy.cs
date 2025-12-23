using System.Collections.Generic;

namespace tech_software_engineer_consultant_int_backend.Models
{
    public class RolePolicy
    {
        public int Id { get; set; }
        public string PolicyName { get; set; }

        // Relation Many-to-Many avec Role
        public ICollection<RolePolicyRole> RolePolicyRoles { get; set; }
    }

}
