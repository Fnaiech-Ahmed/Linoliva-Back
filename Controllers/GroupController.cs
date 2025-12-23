using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Controllers
{
    public class GroupController : ControllerBase
    {
        private readonly MyDbContext _context;

        public GroupController(MyDbContext context)
        {
            _context = context;
        }
    }
}
