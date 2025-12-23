using Microsoft.EntityFrameworkCore;
using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Repositories
{
    public class LicenceRepository : ILicenceRepository
    {
        private readonly MyDbContext _context;

        public LicenceRepository(MyDbContext context)
        {
            _context = context;
        }

        public async Task<Licence> CreateAsync(Licence licence)
        {
            await _context.Set<Licence>().AddAsync(licence);
            await _context.SaveChangesAsync();
            return licence;
        }

        public async Task<Licence> GetByIdAsync(string id)
        {
            return await _context.Set<Licence>().FindAsync(id);
        }

        public async Task<IEnumerable<Licence>> GetAllAsync()
        {
            return await _context.Set<Licence>().ToListAsync();
        }

        public async Task UpdateAsync(Licence licence)
        {
            _context.Set<Licence>().Update(licence);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var licence = await GetByIdAsync(id);
            if (licence != null)
            {
                _context.Set<Licence>().Remove(licence);
                await _context.SaveChangesAsync();
            }
        }
    }

}
