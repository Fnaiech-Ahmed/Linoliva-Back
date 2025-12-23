using System.ComponentModel.DataAnnotations;

namespace tech_software_engineer_consultant_int_backend.Models
{
    public class Impaye
    {
        [Key]
        public int id { get; set; }
        public string Ref { get; set; } = "";
        public string RefClt { get; set; } = "";
        public string Nom { get; set; } = "";
        public string Prenom { get; set; } = "";
        public string Createdate { get; set; } = "";
        public string Datetombe { get; set; } = "";
        public string Retard { get; set; } = "";
        public string Statut { get; set; } = "";
        public string Priorite { get; set; } = "";
        public int NbreImpaye { get; set; }
        public string Montant { get; set; } = "";

        public int? DossierId { get; set; }
        public Dossier? Dossier { get; set; }

        public int? EcheanceId { get; set; }
        public Echeance? Echeance { get; set; }

    }
}