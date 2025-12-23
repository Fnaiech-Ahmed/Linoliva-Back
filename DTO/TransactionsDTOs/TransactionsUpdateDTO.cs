using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.DTO.TransactionsDTOs
{
    public class TransactionsUpdateDTO
    {
        public int ProductId { get; set; }
        public string NomProduit { get; set; }
        public decimal PrixUnitaire { get; set; }
        public int QuantityEntered { get; set; }

        public decimal PTHT { get; set; }
        public decimal TVA { get; set; }
        public decimal PTTC { get; set; }


        public TypeTransaction TypeTransaction { get; set; }



        public static TransactionsUpdateDTO FromTransactionsEntity(Transactions transactions)
        {
            return new TransactionsUpdateDTO
            {
                ProductId = transactions.ProductId,
                NomProduit = transactions.NomProduit,
                PrixUnitaire = transactions.PrixUnitaire,
                QuantityEntered = transactions.QuantityEntered,
                PTHT = transactions. PTHT,
                TVA = transactions.TVA,
                PTTC = transactions.PTTC,
                TypeTransaction = transactions.TypeTransaction
            };
        }


        public Transactions ToTransactionsEntity()
        {
            return new Transactions
            {
                ProductId = ProductId,
                NomProduit = NomProduit,
                PrixUnitaire = PrixUnitaire,
                QuantityEntered = QuantityEntered,
                PTHT = PTHT,
                TVA = TVA,
                PTTC = PTTC,
                TypeTransaction = TypeTransaction
            };
        }
    }
}
