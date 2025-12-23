using tech_software_engineer_consultant_int_backend.Models;
using Microsoft.EntityFrameworkCore;
using tech_software_engineer_consultant_int_backend.Repositories;
using tech_software_engineer_consultant_int_backend.DTO;
using tech_software_engineer_consultant_int_backend.DTO.ImpayesDTOs;
using Microsoft.IdentityModel.Tokens;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public class ImpayeService: IImpayeService
    {
        private readonly MyDbContext _context;
        private readonly IImpayeRepository _ImpayeRepository;
        private readonly IEcheanceService _EcheanceService;

        public ImpayeService(MyDbContext context, IImpayeRepository impayeRepository, IEcheanceService echeanceService)
        {
            _context = context;
            _ImpayeRepository = impayeRepository;
            _EcheanceService = echeanceService;
        }

        public async Task<List<Impaye>> GetImpayesAsync()
        {
            return await _context.Impayes.ToListAsync();
        }

        public async Task<Impaye> GetImpayeAsync(int id)
        {
            return await _context.Impayes.FindAsync(id);
        }

        public async Task<int> CreateImpayeAsync(ImpayeCreateDTO impayeDTO)
        {
            Impaye impaye = impayeDTO.ToImpayeEntity() ;
            Impaye? AddedImpaye = await _ImpayeRepository.AddAsyncImpaye(impaye) ;
            
            return impaye.id;
        }

        public async Task UpdateImpayeAsync(int id, Impaye impaye)
        {
            var existingImpaye = await _context.Impayes.FindAsync(id);

            if (existingImpaye == null)
            {
                throw new ArgumentException("Impaye not found.");
            }

            existingImpaye.Nom = impaye.Nom;

            await _context.SaveChangesAsync();
        }

        public async Task AffectImpayertoDossier(int idImpayer, int idDossier)
        {
            var existingImpayer = _context.Impayes.Find(idImpayer);
            var existingDossier = _context.Dossiers.Find(idDossier);

            if (existingImpayer == null)
            {
                throw new ArgumentException("Imapyer not Found.");
            }
            if (existingDossier == null)
            {
                throw new ArgumentException("Dossier not Found.");
            }

            existingImpayer.DossierId = idDossier;
            existingImpayer.Dossier = existingDossier;

            if (existingDossier.ListeImpayes != null) 
            {
                existingDossier.ListeImpayes.Add(existingImpayer);
            } else
            {
                List<Impaye> impayes = new List<Impaye>();
                impayes.Add(existingImpayer);
                existingDossier.ListeImpayes = impayes;
            }
            
            await _context.SaveChangesAsync();
        }





        public async Task<Impaye> AffectImpayertoEcheance(int idImpayer, int idEcheance)
        {
            var existingImpayer = await _ImpayeRepository.GetImpayeByIdAsync(idImpayer);
            var existingEcheance = await _EcheanceService.GetByIdAsync(idEcheance);

            if (existingImpayer == null)
            {
                //throw new ArgumentException("Imapyer not Found.");
                return new Impaye();
            }
            if (existingEcheance == null)
            {
                //throw new ArgumentException("Echeance not Found.");
                return new Impaye();
            }

            existingImpayer.EcheanceId = idEcheance;
            existingImpayer.Echeance = existingEcheance.ToEcheanceEntity();

            UpdateEcheanceDto updateEcheanceDto = new UpdateEcheanceDto(existingEcheance.ToEcheanceEntity());
            updateEcheanceDto.ImpayeId = idImpayer;

            Impaye? modifiedImpaye = await _ImpayeRepository.UpdateImpaye(existingImpayer);

            bool resultatUpdatedEcheance = await _EcheanceService.UpdateAsync(idEcheance,updateEcheanceDto);

            if ( modifiedImpaye.id.ToString().IsNullOrEmpty() == false || resultatUpdatedEcheance == true)
            {
                return modifiedImpaye;
            }
            return new Impaye();
        }

        public async Task DeleteImpayeAsync(int id)
        {
            var impaye = await _context.Impayes.FindAsync(id);

            if (impaye == null)
            {
                throw new ArgumentException("Impaye not found.");
            }

            _context.Impayes.Remove(impaye);
            await _context.SaveChangesAsync();
        }

    }
}
