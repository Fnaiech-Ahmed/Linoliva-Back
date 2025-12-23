using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.DTO.ProductDTOs
{
    public class ProductCreateDTO
    {
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public string? Barcode { get; set; }
        // Autres propriétés du produit
        public string? Description { get; set; }
        public string? Adresse_Image_Product { get; set; }


        public string? Family { get; set; }
        public string? SubFamily { get; set; }
        public string? Reference_SubFamily { get; set; }

        public string? Category { get; set; }
        public string? Sub_Category { get; set; }





        public ProductCreateDTO() { }


        public ProductCreateDTO(Product product) { 
            this.Name = product.Name;
            this.Price = product.Price;
            this.Barcode = product.Barcode;
            this.Description = product.Description;
            this.Adresse_Image_Product = product.Adresse_Image_Product;

            this.Family = product.Family;
            this.SubFamily = product.SubFamily;

            this.Category = product.Category;
            this.Sub_Category = product.Sub_Category;
        }


        public Product ToProductEntity()
        {
            return new Product
            {
                Name = this.Name,
                Price = this.Price,
                Barcode = this.Barcode,
                Description = this.Description,
                Adresse_Image_Product = this.Adresse_Image_Product,
                Family = this.Family,
                SubFamily = this.SubFamily,
                Category = this.Category,
                Sub_Category = this.Sub_Category,
            };
        }
    }
}
