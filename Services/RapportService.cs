using tech_software_engineer_consultant_int_backend.Models;
using Microsoft.EntityFrameworkCore;


namespace tech_software_engineer_consultant_int_backend.Services
{
    public class RapportService : IRapportService
    {
        private readonly MyDbContext _context;

        public RapportService(MyDbContext context)
        {
            _context = context;
        }

        public async Task<List<Rapport>> GetRapportsAsync()
        {
            return await _context.Rapports.ToListAsync();
        }

        public async Task<Rapport> GetRapportAsync(int id)
        {
            return await _context.Rapports.FindAsync(id);
        }

        public async Task<int> CreateRapportAsync(Rapport rapport)
        {
            _context.Rapports.Add(rapport);
            await _context.SaveChangesAsync();
            return rapport.Id;
        }

        public async Task UpdateRapportAsync(int id, Rapport rapport)
        {
            var existingRapport = await _context.Rapports.FindAsync(id);

            if (existingRapport == null)
            {
                throw new ArgumentException("Rapport not found.");
            }

            existingRapport.Id = rapport.Id;

            await _context.SaveChangesAsync();
        }

        public async Task AssignReportToPortfolio(int idReport, int idPortfolio)
        {            
            var existingRapport = await _context.Rapports.FindAsync(idReport);

            if (existingRapport == null)
            {
                throw new ArgumentException("Rapport not found.");
            }

            var existingPortfolio = await _context.Portefeuilles.FindAsync(idPortfolio);

            if (existingPortfolio == null)
            {
                throw new ArgumentException("Portefeuille not found.");
            }


            existingRapport.PortefeuilleId = existingPortfolio.Id;
            existingRapport.Portefeuille = existingPortfolio;

            if (existingPortfolio.ListeRapports != null) 
            {
                existingPortfolio.ListeRapports.Add(existingRapport);
            }
            else
            {
                List<Rapport> rapports = new List<Rapport>();
                rapports.Add(existingRapport);
                existingPortfolio.ListeRapports = rapports;
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRapportAsync(int id)
        {
            var rapport = await _context.Rapports.FindAsync(id);

            if (rapport == null)
            {
                throw new ArgumentException("Rapport not found.");
            }

            _context.Rapports.Remove(rapport);
            await _context.SaveChangesAsync();
        }

    }
}
