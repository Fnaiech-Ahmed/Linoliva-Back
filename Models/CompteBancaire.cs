using System.ComponentModel.DataAnnotations;

namespace tech_software_engineer_consultant_int_backend.Models
{
    public class CompteBancaire
    {
        [Key]
        public int Id { get; set; }
        public decimal SalaireMensuel { get; set; } // Utilisation de decimal pour représenter des montants monétaires
        public decimal MontantBloque { get; set; }
        public string RIB { get; set; } // Utilisation de string pour représenter le RIB, car certains pays ont des RIB avec des lettres
        public string Segmentation { get; set; }
        public int EngagementTotal { get; set; } // Utilisation de decimal si nécessaire pour représenter des montants monétaires
        public decimal SoldeDisponible { get; set; } // Utilisation de decimal pour représenter des montants monétaires
        public string DeviseCompte { get; set; } // Utilisation de string pour représenter la devise (ex: EUR, USD, etc.)
        public string IBAN { get; set; } // Utilisation de string pour représenter l'IBAN, car il peut contenir des lettres
        public string ClasseDeRisque { get; set; }



        // Constructeur par défaut (vide)
        public CompteBancaire()
        {
        }


        // Constructeur avec paramètres pour les propriétés obligatoires
        public CompteBancaire(decimal salaireMensuel, decimal montantBloque, string rib, string deviseCompte)
        {
            SalaireMensuel = salaireMensuel;
            MontantBloque = montantBloque;
            RIB = rib;
            DeviseCompte = deviseCompte;
        }
    }
}
