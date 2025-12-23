using System;
using System.ComponentModel.DataAnnotations;
using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.DTO
{
    public class CreateEcheanceDto
    {
        [Required]
        public string Ref { get; set; } = "";
        [Required]
        public string Description { get; set; } = "";
        [Required]
        public DateTime DateEcheance { get; set; }
        [Required]
        public double Montant { get; set; }


        //Ensure a default parameterless constructor
        public CreateEcheanceDto() 
        { 
        }



        public CreateEcheanceDto(Echeance echeance)
        {
            Ref = echeance.Ref;
            Description = echeance.Description;
            DateEcheance = echeance.DateEcheance;
            Montant = echeance.Montant;
        }


        // Méthode pour convertir un CreateEcheanceDto en entité Echeance
        public Echeance ToEcheanceEntity()
        {
            return new Echeance
            {
                Ref = Ref,
                Description = Description,
                DateEcheance = DateEcheance,
                Montant = Montant
            };
        }
    }
}
