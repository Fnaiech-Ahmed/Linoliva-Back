using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.DTO.BLDTO
{
    public class BLCreateDTO
    {
        public Titre? TitreClient { get; set; }
        public string NomClient { get; set; }
        public string AdresseClient { get; set; }
        public string MFClient { get; set; }
        public long GSMClient { get; set; }
        public int? ProprietaireId { get; set; }


        public List<Commande> Commandes { get; set; }
        public decimal RemiseBL { get; set; }
        public decimal TVA { get; set; }


        public BLCreateDTO() { 
        
        }

        public BonDeLivraison ToBonDeLivraisonEntity (){
            return new BonDeLivraison
            {
                TitreClient = TitreClient,
                NomClient = NomClient,
                AdresseClient = AdresseClient,
                MFClient = MFClient,
                GSMClient = GSMClient,
                ProprietaireId = ProprietaireId,
                Commandes = Commandes,
                RemiseBL = RemiseBL,
                TVA = TVA
            }; 
        }


        public static BLCreateDTO FromBonDeLivraisonEntity(BonDeLivraison bonDeLivraison)
        {
            return new BLCreateDTO
            {
                TitreClient = bonDeLivraison.TitreClient,
                NomClient = bonDeLivraison.NomClient,
                AdresseClient = bonDeLivraison.AdresseClient,
                MFClient = bonDeLivraison.MFClient,
                GSMClient = bonDeLivraison.GSMClient,
                ProprietaireId = bonDeLivraison.ProprietaireId,
                Commandes = bonDeLivraison.Commandes,
                RemiseBL = bonDeLivraison.RemiseBL,
                TVA = bonDeLivraison.TVA,
            };
        }
    }
}
