using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.DTO.InventaireProduitDTOs
{
    public class InventaireProduitCreateDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }


        public InventaireProduitCreateDTO() { }


        public static InventaireProduitCreateDTO FromInventaireProduitEntity(InventaireProduit inventaireProduit)
        {
            return new InventaireProduitCreateDTO
            {
                ProductId = inventaireProduit.ProductId,
                ProductName = inventaireProduit.ProductName,
                Quantity = inventaireProduit.Quantity
            };
        }


        public InventaireProduit ToInventaireProduitEntity()
        {
            return new InventaireProduit
            {
                ProductId = ProductId,
                ProductName = ProductName,
                Quantity = Quantity,
            };
        }
    }
}
