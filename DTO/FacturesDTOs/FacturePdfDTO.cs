using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.DTO.FacturesDTOs
{
    public class FacturePdfDTO
    {
        // ===== FACTURE (IDENTIQUE À Facture) =====
        public int Id { get; set; }
        public string ReferenceFacture { get; set; }
        public Titre TitreClient { get; set; }
        public string NomClient { get; set; }
        public string AdresseClient { get; set; }
        public string MFClient { get; set; }
        public long GSMClient { get; set; }
        public int? ProprietaireId { get; set; }

        [NotMapped]
        public List<BonDeLivraison> ListeBL { get; set; } = new();

        public DateTime DateEmission { get; set; }
        public DateTime DateMiseAJour { get; set; }

        public decimal MontantHTFacture { get; set; }
        public decimal TVA { get; set; }
        public decimal Remise { get; set; }
        public decimal MontantTTCFacture { get; set; }

        // ===== AJOUTS DEMANDÉS =====
        public string ClientCityZip { get; set; }

        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyPhone { get; set; }

        public string Notes { get; set; }
        public bool IncludeFodec { get; set; }

        // ===== SERIALISATION IDENTIQUE =====
        public string ListeBLSerialized
        {
            get => JsonConvert.SerializeObject(ListeBL);
            set => ListeBL = string.IsNullOrEmpty(value)
                ? new List<BonDeLivraison>()
                : JsonConvert.DeserializeObject<List<BonDeLivraison>>(value);
        }
    }
}