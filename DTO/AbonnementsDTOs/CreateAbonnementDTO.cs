using tech_software_engineer_consultant_int_backend.DTO.ActivationCodesDTOs;

namespace TSE_Consultant_INT_Backend.Models.DTO.AbonnementsDTOs
{
    public class CreateAbonnementDTO : CreateActivationCodeDTO
    {
        public string SubscriptionType { get; set; } // Type d'abonnement (Mensuel, Annuel, etc.)

        //public int? DossierId { get; set; }
    }
}
