using System.ComponentModel.DataAnnotations;

namespace tech_software_engineer_consultant_int_backend.Models
{
    public class Sequence
    {
        [Key]
        public string Name { get; set; }
        public int NextValue { get; set; }
    }
}
