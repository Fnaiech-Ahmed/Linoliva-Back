using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.DTO.FacturesDTOs
{
    public class FactureCreateDTO
    {
        public Titre TitreClient { get; set; }
        public string NomClient { get; set; }
        public string AdresseClient { get; set; }
        public string MFClient { get; set; }
        public long GSMClient { get; set; }
        public int? ProprietaireId { get; set; }

        // Mise à jour de List<BonDeLivraison> en List<string> pour stocker les références
        public List<string> ListeRefsBLs { get; set; }

        public DateTime? DateEmission { get; set; }
        public DateTime? DateMiseAJour { get; set; }

        public decimal TVA { get; set; }
        public decimal Remise { get; set; }

        public FactureCreateDTO()
        {
            ListeRefsBLs = new List<string>();
        }

        public static FactureCreateDTO FromFactureEntity(Facture facture)
        {
            return new FactureCreateDTO
            {
                TitreClient = facture.TitreClient,
                NomClient = facture.NomClient,
                AdresseClient = facture.AdresseClient,
                MFClient = facture.MFClient,
                GSMClient = facture.GSMClient,
                ProprietaireId = facture.ProprietaireId,


                // Transformation des BonDeLivraison en références
                ListeRefsBLs = facture.ListeBL.Select(bl => bl.Reference).ToList(),
                DateEmission = facture.DateEmission,
                DateMiseAJour = facture.DateMiseAJour,


                TVA = facture.TVA,
                Remise = facture.Remise,
            };
        }

        public Facture ToFactureEntity()
        {
            return new Facture
            {
                TitreClient = TitreClient,
                NomClient = NomClient,
                AdresseClient = AdresseClient,
                MFClient = MFClient,
                GSMClient = GSMClient,
                ProprietaireId = ProprietaireId,


                // Vous devrez récupérer les BLs à partir des références avant de les assigner
                ListeBL = new List<BonDeLivraison>(), // Initialisé vide; vous devez le remplir ensuite
                DateMiseAJour = DateMiseAJour ?? DateTime.Now,
                DateEmission = DateEmission ?? DateTime.Now,


                TVA = TVA,
                Remise = Remise,
            };
        }
    }
}
