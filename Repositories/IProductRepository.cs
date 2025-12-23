using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Repository
{
    public interface IProductRepository<T>
    {
        public Task<int> AddProduct(Product product);
        public Task<bool> UpdateProduct(Product product);
        Task DesactiverProduitAsync(int productId);
        public Task<bool> DeleteProduct(int productId);
        public Task<List<Product>> GetProducts();
        public Task<Product?> GetProductById(int id);
        public Task<Product?> GetProductByBarcodeAsync(string barcode);

    }
}
