using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.DTO.BLDTO
{
    public class BLDTO
    {
        public int Id { get; set; }
        public Titre? TitreClient { get; set; }
        public string? NomClient { get; set; }
        public string? AdresseClient { get; set; }
        public string? MFClient { get; set; }
        public long GSMClient { get; set; }
        public int? ProprietaireId { get; set; }

        public List<Commande> Commandes { get; set; }
        public decimal MontantTotalHTBL { get; set; }
        public decimal RemiseBL { get; set; }
        public decimal NetHT { get; set; } //Montant de la BL HT, après Remise
        public decimal TVA { get; set; }
        public decimal MontantTotalTTCBL { get; set; }



        public BonDeLivraison ToBonDeLivraisonEntity()
        {
            return new BonDeLivraison
            {
                Id = Id,
                TitreClient = TitreClient,
                NomClient = NomClient,
                AdresseClient = AdresseClient,
                MFClient = MFClient,
                GSMClient = GSMClient,
                ProprietaireId = ProprietaireId,
                Commandes = Commandes,
                RemiseBL = RemiseBL,
                TVA = TVA,
                MontantTotalHTBL = MontantTotalHTBL,
                NetHT = NetHT,
                MontantTotalTTCBL = MontantTotalTTCBL,
            };
        }


        public static BLDTO FromBonDeLivraisonEntity(BonDeLivraison bonDeLivraison)
        {
            return new BLDTO
            {
                Id = bonDeLivraison.Id,
                TitreClient = bonDeLivraison.TitreClient,
                NomClient = bonDeLivraison.NomClient,
                AdresseClient = bonDeLivraison.AdresseClient,
                MFClient = bonDeLivraison.MFClient,
                GSMClient = bonDeLivraison.GSMClient,
                ProprietaireId = bonDeLivraison.ProprietaireId,
                Commandes = bonDeLivraison.Commandes,
                RemiseBL = bonDeLivraison.RemiseBL,
                TVA = bonDeLivraison.TVA,
                MontantTotalHTBL = bonDeLivraison.MontantTotalHTBL,
                NetHT = bonDeLivraison.NetHT,
                MontantTotalTTCBL = bonDeLivraison.MontantTotalTTCBL,
            };
        }
    }
}
