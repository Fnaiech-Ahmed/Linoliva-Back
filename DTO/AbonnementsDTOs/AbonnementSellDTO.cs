using tech_software_engineer_consultant_int_backend.DTO.ActivationCodesDTOs;

namespace tech_software_engineer_consultant_int_backend.DTO.AbonnementsDTOs
{
    public class AbonnementSellDTO : ActivationCodeSellDTO
    {
        public bool IsRecurring { get; set; }   // Indique si l'abonnement est récurrent (renouvelable automatiquement).
                                                // Activé pour des secteurs comme le secteur Sanitaire.
        public bool IsActive { get; set; } // Statut actif ou non
        public string PaymentMethod { get; set; } // Méthode de paiement
    }
}
