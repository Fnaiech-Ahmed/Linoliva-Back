using System;
using System.ComponentModel.DataAnnotations;
using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.DTO
{
    public class UpdateEcheanceImpayeIdDto
    {
        [Required]               
        public int? ImpayeId { get; set; }
        [Required]
        public Impaye? Impaye { get; set; }


        public UpdateEcheanceImpayeIdDto() { }


        public UpdateEcheanceImpayeIdDto(Echeance echeance)
        {
            ImpayeId = echeance.ImpayeId;
            Impaye = echeance.Impaye;
        }


        public Echeance ToEcheanceEntity()
        {
            return new Echeance
            {
                Impaye = Impaye,
                ImpayeId = ImpayeId
            };
        }
    }
}
