using System;
using System.ComponentModel.DataAnnotations;

namespace tech_software_engineer_consultant_int_backend.Models
{
    public class Echeance
    {
        [Key]
        public int Id { get; set; }
        public string Ref { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime DateEcheance { get; set; }
        public double Montant { get; set; }

        public int? ImpayeId { get; set; }
        public Impaye? Impaye { get; set; }


        //Constructeur par défault (vide)
        public Echeance() 
        { 
        }



    }
}
