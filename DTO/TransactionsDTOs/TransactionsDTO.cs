using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.DTO.TransactionsDTOs
{
    public class TransactionsDTO
    {
        public int Id { get; set; }
        public string ReferenceCommande { get; set; }
        public string ReferenceLot { get; set; }
        public int ProductId { get; set; }
        public string NomProduit { get; set; }
        public decimal PrixUnitaire { get; set; }
        public int QuantityEntered { get; set; }

        public decimal PTHT { get; set; }
        public decimal TVA { get; set; }
        public decimal PTTC { get; set; }


        public TypeTransaction TypeTransaction { get; set; }

        public int RemainingQuantity { get; set; }


        public TransactionsDTO()
        {

        }


        public TransactionsDTO(Transactions transactions)
        {
            Id = transactions.Id;
            ReferenceCommande = transactions.ReferenceCommande;
            ReferenceLot = transactions.ReferenceLot;
            ProductId = transactions.ProductId;
            NomProduit = transactions.NomProduit;
            PrixUnitaire = transactions.PrixUnitaire;
            QuantityEntered = transactions.QuantityEntered;
            PTHT = transactions.PTHT;
            TVA = transactions.TVA;
            PTTC = transactions.PTTC;
            TypeTransaction = transactions.TypeTransaction;
            RemainingQuantity = transactions.RemainingQuantity;
        }


        public Transactions ToTransactionsEntity()
        {
            return new Transactions() {
                Id = Id,
                ReferenceCommande = ReferenceCommande,
                ReferenceLot = ReferenceLot,
                ProductId = ProductId,
                NomProduit = NomProduit,
                PrixUnitaire = PrixUnitaire, 
                QuantityEntered = QuantityEntered, 
                PTHT = PTHT, 
                TVA = TVA, 
                PTTC = PTTC,
                TypeTransaction = TypeTransaction,
                RemainingQuantity = RemainingQuantity
            };
        }
    }
}
