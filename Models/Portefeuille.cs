namespace tech_software_engineer_consultant_int_backend.Models
{
    public class Portefeuille
    {
        public int Id { get; set; }
        public string? Nom { get; set; }
        public int? NbrDossiers { get; set; }
        public string? Date { get; set; }
        public string? Action { get; set; }

        public List<Dossier>? ListeDossiers { get; set; }

        public List<Rapport>? ListeRapports { get; set; }

        public List<UserPortefeuilleAssociation>? UserPortefeuilleAssociations { get; set; }

    }
}
