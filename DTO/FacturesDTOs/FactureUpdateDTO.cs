using tech_software_engineer_consultant_int_backend.Models;
using System;
using System.Collections.Generic;
using tech_software_engineer_consultant_int_backend.Services;

namespace tech_software_engineer_consultant_int_backend.DTO.FacturesDTOs
{
    public class FactureUpdateDTO
    {
        public Titre TitreClient { get; set; }
        public string NomClient { get; set; }
        public string AdresseClient { get; set; }
        public string MFClient { get; set; }
        public long GSMClient { get; set; }

        public List<string> ListeRefsBLs { get; set; }

        public DateTime? DateEmission { get; set; }
        public DateTime? DateMiseAJour { get; set; }

        public decimal TVA { get; set; }
        public decimal Remise { get; set; }

        public FactureUpdateDTO() { }

        public static FactureUpdateDTO FromFactureEntity(Facture entity)
        {
            return new FactureUpdateDTO()
            {
                TitreClient = entity.TitreClient,
                NomClient = entity.NomClient,
                AdresseClient = entity.AdresseClient,
                MFClient = entity.MFClient,
                GSMClient = entity.GSMClient,
                ListeRefsBLs = entity.ListeBL.Select(bl => bl.Reference).ToList(), // Assuming BonDeLivraison has a 'Reference' property
                DateEmission = entity.DateEmission,
                DateMiseAJour = entity.DateMiseAJour,
                TVA = entity.TVA,
                Remise = entity.Remise,
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
                // Vous devrez récupérer les BLs à partir des références avant de les assigner
                ListeBL = new List<BonDeLivraison>(), // Initialisé vide; vous devez le remplir ensuite
                DateEmission = DateEmission ?? DateTime.MinValue,  // Use a default value if DateEmission is null
                DateMiseAJour = DateMiseAJour ?? DateTime.MinValue, // Use a default value if DateMiseAJour is null
                TVA = TVA,
                Remise = Remise,
            };
        }

    }
}
