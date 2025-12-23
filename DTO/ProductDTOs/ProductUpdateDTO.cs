using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.DTO.ProductDTOs
{
    public class ProductUpdateDTO
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



        public ProductUpdateDTO() { }


        public static ProductUpdateDTO FromProductEntity(Product product)
        {
            return new ProductUpdateDTO
            {
                Name = product.Name,
                Price = product.Price,
                Barcode = product.Barcode,
                Description = product.Description,
                Adresse_Image_Product = product.Adresse_Image_Product,

                Family = product.Family,
                SubFamily = product.SubFamily,

                Category = product.Category,
                Sub_Category = product.Sub_Category,
            };
        }


        public Product ToProductEntity()
        {
            return new Product
            {
                Name = Name,
                Price = Price,
                Barcode = Barcode,
                Description = Description,
                Adresse_Image_Product = Adresse_Image_Product,
                Family = Family,
                SubFamily = SubFamily,
                Category = Category,
                Sub_Category = Sub_Category,
            };
        }
    }
}
