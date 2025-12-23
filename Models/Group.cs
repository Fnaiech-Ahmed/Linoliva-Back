using System.ComponentModel.DataAnnotations;

namespace tech_software_engineer_consultant_int_backend.Models
{
    public class Group
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public List<UserGroupAssociation>? UserGroupAssociations { get; set; }

        public List<Message>? MessagesList { get; set; }
    }

}
