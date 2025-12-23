using tech_software_engineer_consultant_int_backend.DTO.ProductDTOs;
using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public interface IProductService
    {
        Task<bool> AddProduct(ProductCreateDTO productCreateDTO);
        Task<List<ProductDTO>> GetProducts();
        Task<ProductDTO?> GetProductById(int id);
        Task<ProductDTO?> GetProductByBarCode(string CodeABarre);
        Task<bool> UpdateProduct(int productId, ProductUpdateDTO productUpdateDTO);
        Task DesactiverProduitAsync(int productId);
        Task<bool> DeleteProduct(int id);

    }
}
