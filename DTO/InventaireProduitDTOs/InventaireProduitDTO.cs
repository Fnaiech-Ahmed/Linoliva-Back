using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.DTO.InventaireProduitDTOs
{
    public class InventaireProduitDTO
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }



        public InventaireProduitDTO() { }


        public static InventaireProduitDTO FromInvetaireProduitEntity(InventaireProduit entity)
        {
            return new InventaireProduitDTO
            {
                Id = entity.Id,
                ProductId = entity.ProductId,
                ProductName = entity.ProductName,
                Quantity = entity.Quantity
            };
        }


        public InventaireProduit ToInventaireProduitEntity()
        {
            return new InventaireProduit { 
                Id = Id, 
                ProductId = ProductId,
                ProductName = ProductName,
                Quantity = Quantity,
            };
        }
    }
}
