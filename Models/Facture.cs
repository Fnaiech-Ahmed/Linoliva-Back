using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tech_software_engineer_consultant_int_backend.Models
{
    public class Facture
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string ReferenceFacture {  get; set; }
        public Titre TitreClient { get; set; }
        public string NomClient { get; set; }
        public string AdresseClient { get; set; }
        public string MFClient { get; set; }
        public long GSMClient { get; set; }
        public int? ProprietaireId { get; set; }

        [NotMapped]
        public List<BonDeLivraison> ListeBL {  get; set; }

        public DateTime DateEmission { get; set; }
        public DateTime DateMiseAJour { get; set; }

        public decimal MontantHTFacture { get; set; }
        public decimal TVA { get; set; }
        public decimal Remise {  get; set; }
        public decimal MontantTTCFacture { get; set; }



        // Property for serialized ListeBL
        public string ListeBLSerialized
        {
            get => SerializeListeBL(ListeBL);
            set => ListeBL = DeserializeListeBL(value);
        }

        public Facture()
        {
            ListeBL = new List<BonDeLivraison>();
        }



        private string SerializeListeBL(List<BonDeLivraison> listeBL)
        {
            return JsonConvert.SerializeObject(listeBL);
        }

        private List<BonDeLivraison> DeserializeListeBL(string json)
        {
            return JsonConvert.DeserializeObject<List<BonDeLivraison>>(json);
        }
    }
}
