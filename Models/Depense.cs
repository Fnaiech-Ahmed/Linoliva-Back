namespace tech_software_engineer_consultant_int_backend.Models
{
    public class Depense : TransactionFinancière
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }
    }
}
