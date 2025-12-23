using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tech_software_engineer_consultant_int_backend.Models
{
    public class Lot
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Reference { get; set; }
        public int IDProduit { get; set; }
        public int Quantite {  get; set; }

        public DateTime Date { get; set; }
        public Lot() { }
    }
}
