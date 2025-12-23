using System.ComponentModel.DataAnnotations;

namespace tech_software_engineer_consultant_int_backend.Models
{
    public class LotsTransactions
    {
        [Key]
        public int Id { get; set; }
        public string IdTransaction { get; set; }
        public string RefLot { get; set; }
        public int Quantite { get; set; }
        public LotsTransactions() { }
    }
}
