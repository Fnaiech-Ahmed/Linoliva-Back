using tech_software_engineer_consultant_int_backend.DTO.ImpayesDTOs;
using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.DTO
{
    public class EcheanceDto
    {
        public int Id { get; set; }
        public string Ref { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime DateEcheance { get; set; }
        public double Montant { get; set; }
        public int? ImpayeId { get; set; }



        public EcheanceDto() 
        { 
        }


        public EcheanceDto(Echeance echeance)
        {
            Id = echeance.Id;
            Ref = echeance.Ref;
            Description = echeance.Description;
            DateEcheance = echeance.DateEcheance;
            Montant = echeance.Montant;
            ImpayeId = echeance.ImpayeId;
        }


        // Méthode pour convertir un EcheanceDto en entité Echeance
        public Echeance ToEcheanceEntity()
        {
            return new Echeance
            {
                Id = Id,
                Ref = Ref,
                Description = Description,
                DateEcheance = DateEcheance,
                Montant = Montant,
                ImpayeId = ImpayeId,
            };
        }
    }
}
