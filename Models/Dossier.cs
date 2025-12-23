using System.ComponentModel.DataAnnotations;

namespace tech_software_engineer_consultant_int_backend.Models
{
    public class Dossier
    {
        [Key]
        public int Id { get; set; }

        public int? PortefeuilleId { get; set; }  // Propriété pour stocker la clé étrangère
        public Portefeuille? Portefeuille { get; set; }

        public List<Impaye>? ListeImpayes { get; set; }


        public List<Abonnement>? ListeAbonnements { get; set; }

    }
}