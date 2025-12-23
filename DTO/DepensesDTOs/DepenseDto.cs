using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.DTO.DepensesDTOs
{
    public class DepenseDto
    {
        public decimal Montant { get; set; }
        public DateTime DateTransaction { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }

        // Propriété de navigation vers RapportJournalier
        public int? RapportJournalierId { get; set; }


        // Méthode de transformation vers DepenseDto
        public static DepenseDto ToDto(Depense depense)
        {
            return new DepenseDto
            {
                Montant = depense.Montant,
                DateTransaction = depense.DateTransaction,
                ProductId = depense.ProductId,
                ProductName = depense.ProductName,
                UnitPrice = depense.UnitPrice,
                Quantity = depense.Quantity,
                RapportJournalierId = depense.RapportJournalierId
            };
        }

        // Méthode de transformation depuis DepenseDto
        public Depense ToDepenseEntity()
        {
            return new Depense
            {
                Montant = Montant,
                DateTransaction = DateTransaction,
                ProductId = ProductId,
                ProductName = ProductName,
                UnitPrice = UnitPrice,
                Quantity = Quantity,
                RapportJournalierId = RapportJournalierId
            };
        }
    }
}
