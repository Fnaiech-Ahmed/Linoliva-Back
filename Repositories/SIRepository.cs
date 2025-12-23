using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Repository
{
    public class SIRepository : ISIRepository<SI>
    {
        private readonly MyDbContext _context;

        public SIRepository(MyDbContext context)
        {
            _context = context;
        }

        public async Task<List<SI>> GetSIs()
        {
            return await _context.SIs.ToListAsync();
        }

        public async Task<List<(int Id, string Name)>> GetSIsNamesAsync()
        {
            return await _context.SIs
                .Select(p => new { p.Id, p.Name })
                .ToListAsync()
                .ContinueWith(t => t.Result.Select(p => (p.Id, p.Name)).ToList());
        }

        public async Task<int> AddSI(SI si)
        {
            await _context.SIs.AddAsync(si);
            await _context.SaveChangesAsync();
            return si.Id;
        }

        public async Task<SI?> GetSIById(int SIId)
        {
            return await _context.SIs.FindAsync(SIId);
        }

        

        public async Task<bool> UpdateSI(SI si)
        {
            _context.SIs.Update(si);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteSI(int SIId)
        {
            var si = await _context.SIs.FindAsync(SIId);
            if (si == null) return false;

            _context.SIs.Remove(si);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
