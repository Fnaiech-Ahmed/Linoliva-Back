using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tech_software_engineer_consultant_int_backend.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public string? Barcode { get; set; }
        // Autres propriétés du produit
        public string? Description { get; set; }
        public string? Adresse_Image_Product { get; set; }

        public string Family { get; set; }

        public string SubFamily { get; set; }

        public string Category { get; set; }

        public string Sub_Category { get; set; }
        public bool? Actif { get; set; }

    }
}
