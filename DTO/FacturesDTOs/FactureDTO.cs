using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.DTO.FacturesDTOs
{
    public class FactureDTO
    {
        public int Id { get; set; }
        public string ReferenceFacture { get; set; }
        public Titre TitreClient { get; set; }
        public string NomClient { get; set; }
        public string AdresseClient { get; set; }
        public string MFClient { get; set; }
        public long GSMClient { get; set; }
        public int? ProprietaireId { get; set; }

        public List<BonDeLivraison> ListeBL { get; set; }

        public DateTime? DateEmission { get; set; }
        public DateTime? DateMiseAJour { get; set; }

        public decimal MontantHTFacture { get; set; }
        public decimal TVA { get; set; }
        public decimal Remise { get; set; }
        public decimal MontantTTCFacture { get; set; }



        public FactureDTO() { }


        public static FactureDTO FromFactureEntity(Facture entity)
        {
            return new FactureDTO { 
                Id = entity.Id,
                ReferenceFacture = entity.ReferenceFacture,

                TitreClient = entity.TitreClient,
                NomClient = entity.NomClient,
                AdresseClient = entity.AdresseClient,
                MFClient = entity.MFClient,
                GSMClient = entity.GSMClient,
                ProprietaireId = entity.ProprietaireId,

                ListeBL = entity.ListeBL,
                DateEmission = entity.DateEmission,
                DateMiseAJour = entity.DateMiseAJour,

                MontantHTFacture = entity.MontantHTFacture,
                TVA = entity.TVA,
                Remise = entity.Remise,
                MontantTTCFacture = entity.MontantTTCFacture,
            };
        }


        public Facture ToFactureEntity() {
            return new Facture
            {
                Id = Id,
                ReferenceFacture = ReferenceFacture,

                TitreClient = TitreClient,
                NomClient = NomClient,
                AdresseClient = AdresseClient,
                MFClient = MFClient,
                GSMClient = GSMClient,
                ProprietaireId = ProprietaireId,

                ListeBL = ListeBL,
                DateEmission = (DateTime)DateEmission,
                DateMiseAJour = (DateTime)DateMiseAJour,

                TVA = TVA,
                Remise = Remise,
                MontantHTFacture = MontantHTFacture,
                MontantTTCFacture= MontantTTCFacture,
            };
        }
    }
}
