using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace tech_software_engineer_consultant_int_backend.Models
{
    public class UserRole : IdentityUserRole<int>
    {
        // Pas besoin de redéfinir UserId et RoleId ici
        // Ils sont déjà définis dans IdentityUserRole<int>

        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }
}
