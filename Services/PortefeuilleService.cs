using tech_software_engineer_consultant_int_backend.DTO.PortfoliosDTOs;
using tech_software_engineer_consultant_int_backend.Models;
using tech_software_engineer_consultant_int_backend.Repositories;
using Microsoft.EntityFrameworkCore;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public class PortefeuilleService:IPortefeuilleService
    {
        private readonly MyDbContext _context;
        private readonly IPortefeuilleRepository<Portefeuille> portefeuilleRepository;
        private readonly IUserPortefeuilleAssociationService userPortefeuilleAssociationService;

        public PortefeuilleService(
            MyDbContext context,
            IPortefeuilleRepository<Portefeuille> _portefeuilleRepository,
            IUserPortefeuilleAssociationService _userPortefeuilleAssociationService
            )
        {
            _context = context;
            portefeuilleRepository = _portefeuilleRepository;
            userPortefeuilleAssociationService = _userPortefeuilleAssociationService;
        }

        public async Task<List<Portefeuille>> GetPortefeuillesAsync()
        {
            return await _context.Portefeuilles.ToListAsync();
        }

        public async Task<Portefeuille?> GetPortefeuilleAsync(int id)
        {
            return await _context.Portefeuilles.FindAsync(id);
        }

        public async Task<(bool,int,List<(string,string)>)> CreatePortefeuilleAsync(PortefeuilleCreateDTO portefeuilleCreateDTO, List<string> listeREFsRecouvreurs)
        {
            // Convertir PortefeuilleCreateDTO en une instance de type Portefeuille
            Portefeuille portefeuille = portefeuilleCreateDTO.ToPortefeuilleEntity();

            // Ajouter l'instance portefeuille à la Base des données à travers
            // l'instance de type PortefeuilleRepository
            (bool success,Portefeuille portefeuille) validateAjout = await portefeuilleRepository.AddAsync(portefeuille);

            if (validateAjout.success)
            {
                List<(string, string)> values = new List<(string, string)>();
                foreach(string referenceRecouvreur in listeREFsRecouvreurs)
                {
                    //await AffectRecouvreurToPortefeuille(portefeuille, referenceRecouvreur);
                    (bool success, string title, string message, Portefeuille portefeuille) resultatAjout = await userPortefeuilleAssociationService.AddUserPortefeuilleAssociation(referenceRecouvreur, portefeuille);
                    await portefeuilleRepository.Update(portefeuille);
                    values.Add((resultatAjout.title, resultatAjout.message));                    
                }

                return (validateAjout.success, portefeuille.Id, values);
            }
            else
            {
                List<(string, string)> values = new List<(string, string)> { ("Echoue d'ajout.", "Le système n'a rien ajouté.") };
                return (false, 0, values);
            }
        }


/*        public async Task<bool> AffectRecouvreurToPortefeuille(Portefeuille portefeuille, string referenceRecouvreur)
        {
            RecouvreurDto existingRecouvreur = await _recouvreurService.GetRecouvreurAsync(referenceRecouvreur);

            if (existingRecouvreur != null)
            {
                List<UserPortefeuilleAssociation>? listePortefeuilleUsers = portefeuille.UserPortefeuilleAssociations;
                if (listePortefeuilleUsers == null)
                {
                    List<UserPortefeuilleAssociation> nvlUserPortefeuilleAssociations = new List<UserPortefeuilleAssociation> ();

                }
                return true;
            }
            else
                return false;             

        }
*/
        public async Task UpdatePortefeuilleAsync(int id, Portefeuille portefeuille)
        {
            var existingPortefeuille = await _context.Portefeuilles.FindAsync(id);

            if (existingPortefeuille == null)
            {
                throw new ArgumentException("Portefeuille not found.");
            }

            existingPortefeuille.Nom = portefeuille.Nom;

            await _context.SaveChangesAsync();
        }


        public async Task<(bool, string)> UpdatePortefeuilleUsersAsync(int id, Portefeuille portefeuille)
        {
            var existingPortefeuille = await _context.Portefeuilles.FindAsync(id);

            if (existingPortefeuille == null)
            {
                //throw new ArgumentException("Portefeuille not found.");
                return (false, "Portefeuille not found.");
            }

            existingPortefeuille.UserPortefeuilleAssociations = portefeuille.UserPortefeuilleAssociations;

            bool successUpdate = await portefeuilleRepository.Update(existingPortefeuille);

            if (successUpdate)
            {
                return (true, "Liste des utilisateurs du portefeuille a été mise à jour.");
            }
            else 
            {
                return (false, "Liste des utilisateurs du portefeuille n'a pas été mise à jour.");
            }
        }

        public async Task DeletePortefeuilleAsync(int id)
        {
            var portefeuille = await _context.Portefeuilles.FindAsync(id);

            if (portefeuille == null)
            {
                throw new ArgumentException("Portefeuille not found.");
            }

            _context.Portefeuilles.Remove(portefeuille);
            await _context.SaveChangesAsync();
        }

    }
}
