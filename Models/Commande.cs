using tech_software_engineer_consultant_int_backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace tech_software_engineer_consultant_int_backend.Models
{
    public class Commande
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string ReferenceCommande { get; set; }

        public string NomVendeur { get; set; }
        public string AdresseVendeur { get; set; }
        public string MFVendeur { get; set; }
        public long GSMVendeur { get; set; }
        public int? ProprietaireId { get; set; }

        public Titre TitreAcheteur { get; set; }
        public string NomAcheteur { get; set; }
        public string AdresseAcheteur { get; set; }
        public string MFAcheteur { get; set; }
        public long GSMAcheteur { get; set; }

        public List<int> ListIdsTransactions { get; set; }
        
        public decimal MontantTotalHT { get; set; }
        public decimal TVA { get; set; }
        public decimal MontantTotalTTC { get; set; }

        public Commande() 
        {
        }
    }
}
