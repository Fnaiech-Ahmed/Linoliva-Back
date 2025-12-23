using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace tech_software_engineer_consultant_int_backend.Models
{
    // Table de jonction pour la relation Many-to-Many
    public class RolePolicyRole
    {
        [Key, Column(Order = 0)]
        [ForeignKey(nameof(Role))]
        public int RoleId { get; set; }
        public Role Role { get; set; }

        [Key, Column(Order = 1)]
        [ForeignKey(nameof(RolePolicy))]
        public int RolePolicyId { get; set; }
        public RolePolicy RolePolicy { get; set; }
    }
}
