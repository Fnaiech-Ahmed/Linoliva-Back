using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using tech_software_engineer_consultant_int_backend.DTOs.ProductsDTOs;

namespace tech_software_engineer_consultant_int_backend.Models
{
    public class Transactions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string ReferenceCommande { get; set; }
        public string ReferenceLot { get; set; }
        public int ProductId { get; set; }
        public string NomProduit {  get; set; }
        public decimal PrixUnitaire { get; set; }
        public int QuantityEntered { get; set; }

        public decimal PTHT { get; set; }
        public decimal TVA { get; set; }
        public decimal PTTC { get; set; }


        public TypeTransaction TypeTransaction { get; set; }
        public int RemainingQuantity { get; set; }
        
        


        public Transactions() { }
    }
}
