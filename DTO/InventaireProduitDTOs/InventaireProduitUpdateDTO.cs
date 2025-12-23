using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.DTO.InventaireProduitDTOs
{
    public class InventaireProduitUpdateDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }


        public InventaireProduitUpdateDTO() { }


        public static InventaireProduitUpdateDTO FromInventaireProduitEntity(InventaireProduit entity)
        {
            return new InventaireProduitUpdateDTO { 
                ProductId = entity.ProductId, 
                ProductName = entity.ProductName, 
                Quantity = entity.Quantity 
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
