using tech_software_engineer_consultant_int_backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tech_software_engineer_consultant_int_backend.Repository;
using tech_software_engineer_consultant_int_backend.DTO.ProductDTOs;
using tech_software_engineer_consultant_int_backend.DTO.InventaireProduitDTOs;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository<Product> productRepository;
        private readonly IInventaireProduitService inventaireProduitService;

        public ProductService(IProductRepository<Product> repository, IInventaireProduitService _inventaireProduitService)
        {
            productRepository = repository;
            inventaireProduitService = _inventaireProduitService;
        }

        public async Task<bool> AddProduct(ProductCreateDTO productCreateDTO)
        {
            Product product = productCreateDTO.ToProductEntity();
            int productId = await productRepository.AddProduct(product);
            if (productId > 0)
            {
                InventaireProduitCreateDTO inventaireProduit = new InventaireProduitCreateDTO();
                inventaireProduit.ProductId = productId;
                inventaireProduit.ProductName = product.Name;
                inventaireProduit.Quantity = 0;
                await inventaireProduitService.AddInventaireProduit(inventaireProduit);
                return true;
            }
            return false;
        }

        public async Task<List<ProductDTO>> GetProducts()
        {
            List<Product> products = await productRepository.GetProducts();
            if (products == null || products.Count == 0)
            {
                return new List<ProductDTO>();
            }
            else
            {
                List<ProductDTO> ProductsDTO = new List<ProductDTO>();
                foreach (Product product in products)
                {
                    ProductDTO productDTO = ProductDTO.FromProductEntity(product);
                    ProductsDTO.Add(productDTO);
                }
                return ProductsDTO;
            }
            
        }

        public async Task<ProductDTO?> GetProductById(int id)
        {
            Product? existingProduct = await productRepository.GetProductById(id);

            if (existingProduct == null)
            {
                return new ProductDTO();
            }
            else
            {
                return ProductDTO.FromProductEntity(existingProduct);
            }
            
        }

        public async Task<ProductDTO?> GetProductByBarCode(string CodeABarre)
        {
            Product? existingProduct = await productRepository.GetProductByBarcodeAsync(CodeABarre);
            if (existingProduct == null)
            {
                return new ProductDTO();
            }
            else
            {
                return ProductDTO.FromProductEntity(existingProduct);
            }
            
        }

        public async Task<bool> UpdateProduct(int productId, ProductUpdateDTO productUpdateDTO)
        {
            Product? existingProduct = await productRepository.GetProductById(productId);

            if (existingProduct != null)
            {
                Product updatedProduct = productUpdateDTO.ToProductEntity();
                updatedProduct.Id = productId;
                return await productRepository.UpdateProduct(updatedProduct);  // Appel de la méthode UpdateProduct correcte
            }
            else
            {
                //MessageBox.Show("Le produit est null. Impossible de mettre à jour.");
                return false;
            }
        }

        public async Task DesactiverProduitAsync(int productId)
        {
            // Appel au repository pour désactiver le produit
            await productRepository.DesactiverProduitAsync(productId);
        }

        public async Task<bool> DeleteProduct(int id)
        {
            return await productRepository.DeleteProduct(id);
        }
    }
}
