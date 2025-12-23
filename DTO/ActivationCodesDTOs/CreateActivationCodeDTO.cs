namespace tech_software_engineer_consultant_int_backend.DTO.ActivationCodesDTOs
{
    public class CreateActivationCodeDTO
    {
        public int ProductId { get; set; } // A ne pas toucher, valeur affectée lors de la première création du code d'activation

        public int MaxUsersAllowed { get; set; } // Nombre maximum d'utilisateurs autorisés

        public int MaxDevicesAllowed { get; set; } // Nombre maximum d'appareils autorisés
    }
}
