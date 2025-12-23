using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Models
{
    public class BonDeLivraison
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Reference { get; set; }
        public Titre? TitreClient { get; set; }
        public string NomClient { get; set; }
        public string AdresseClient { get; set; }
        public string MFClient { get; set; }
        public long GSMClient { get; set; }
        public int? ProprietaireId { get; set; }

        //public List<Commande> Commandes { get; set; }


        // Propriété ajoutée
        public string CommandesJson { get; set; } = "[]";

        // Propriété pour manipuler les commandes en tant que liste
        [NotMapped]
        public List<Commande> Commandes
        {
            get => DeserializeCommandes(CommandesJson);
            set => CommandesJson = SerializeCommandes(value);
        }



        public decimal MontantTotalHTBL { get; set; }
        public decimal RemiseBL { get; set; }
        public decimal NetHT { get; set; } //Montant de la BL HT, après Remise
        public decimal TVA { get; set; }
        public decimal MontantTotalTTCBL { get; set; }

        

        // Constructeur
        public BonDeLivraison()
        {
        }

        private string SerializeCommandes(List<Commande> commandes)
        {
            return JsonConvert.SerializeObject(commandes);
        }

        private List<Commande> DeserializeCommandes(string json)
        {
            return string.IsNullOrEmpty(json) ? new List<Commande>() : JsonConvert.DeserializeObject<List<Commande>>(json);
        }

        
    }
}
