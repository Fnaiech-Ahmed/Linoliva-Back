using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Repository
{
    public class ProductRepository : IProductRepository<Product>
    {
        private readonly MyDbContext _context;

        public ProductRepository(MyDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<List<(int Id, string Name)>> GetProductsNamesAsync()
        {
            return await _context.Products
                .Select(p => new { p.Id, p.Name })
                .ToListAsync()
                .ContinueWith(t => t.Result.Select(p => (p.Id, p.Name)).ToList());
        }

        public async Task<int> AddProduct(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product.Id;
        }

        public async Task<Product?> GetProductById(int productId)
        {
            return await _context.Products.FindAsync(productId);
        }

        public async Task<Product?> GetProductByBarcodeAsync(string barcode)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.Barcode == barcode);
        }

        public async Task<bool> UpdateProduct(Product product)
        {
            _context.Products.Update(product);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task DesactiverProduitAsync(int productId)
        {
            // Récupérer le produit à partir de l'Id
            var produit = await _context.Products.FindAsync(productId);

            // Vérifier si le produit existe
            if (produit == null)
            {
                throw new KeyNotFoundException($"Le produit avec l'Id {productId} n'existe pas.");
            }

            // Mettre le champ IsActive sur false
            produit.Actif = false;

            // Marquer l'entité comme modifiée
            _context.Products.Update(produit);

            // Enregistrer les changements dans la base de données
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteProduct(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return false;

            _context.Products.Remove(product);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
