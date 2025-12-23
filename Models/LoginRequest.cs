using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace tech_software_engineer_consultant_int_backend.Models
{
    [Keyless]
    public class LoginRequest
    {
        //public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}