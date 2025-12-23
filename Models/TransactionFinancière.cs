using System.Text.Json.Serialization;


namespace tech_software_engineer_consultant_int_backend.Models
{
    public abstract class TransactionFinancière
    {
        public int Id { get; set; }
        public DateTime DateTransaction { get; set; }
        public decimal Montant { get; set; }

        // Propriété de navigation vers RapportJournalier
        public int? RapportJournalierId { get; set; }

        [JsonIgnore] // Cela exclut rapportJournalier de la sérialisation JSON
        public RapportJournalier RapportJournalier { get; set; }
    }

}
