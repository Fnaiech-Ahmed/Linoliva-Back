using System;
using System.ComponentModel.DataAnnotations;
using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.DTO
{
    public class UpdateEcheanceDto
    {
        [Required]
        public string Ref { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public DateTime DateEcheance { get; set; }
        [Required]
        public double Montant { get; set; }
        public int? ImpayeId { get; set; }
        


        public UpdateEcheanceDto() 
        { 
        }


        public UpdateEcheanceDto(Echeance echeance)
        {
            Ref = echeance.Ref;
            Description = echeance.Description;
            DateEcheance = echeance.DateEcheance;
            Montant = echeance.Montant;
            ImpayeId = echeance.ImpayeId;
           
        }


        public Echeance ToEcheanceEntity()
        {
            return new Echeance
            {
                Ref = Ref,
                Description = Description,
                DateEcheance = DateEcheance,
                Montant = Montant,               
                ImpayeId = ImpayeId
            };
        }
    }
}
