using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.DTO.LotsTransactionsDTOs
{
    public class LotsTransactionsDTO
    {
        public int Id { get; set; }
        public string IdTransaction { get; set; }
        public string RefLot { get; set; }
        public int Quantite { get; set; }

        public static LotsTransactionsDTO FromEntity(LotsTransactions entity)
        {
            return new LotsTransactionsDTO
            {
                Id = entity.Id,
                IdTransaction = entity.IdTransaction,
                RefLot = entity.RefLot,
                Quantite = entity.Quantite
            };
        }

        public LotsTransactions ToEntity()
        {
            return new LotsTransactions
            {
                Id = this.Id,
                IdTransaction = this.IdTransaction,
                RefLot = this.RefLot,
                Quantite = this.Quantite
            };
        }
    }
}
