namespace tech_software_engineer_consultant_int_backend.DTO.ActivationCodesDTOs
{
    public class ActivationCodeSellDTO
    {
        // Informations de la Référence de copie du produit vendue
        public string? ProductReference { get; set; } // Référence du produit auquel le code est affecté


        // Informations du Client
        public string? ClientRef { get; set; } // Identifiant du client


        //Informations des délais de vie du code d'activation
        public DateTime? ActivationDate { get; set; } // Date de début de Code d'activation
        public DateTime? ExpirationDate { get; set; } // Date de fin de Code d'activation
                
    }
}
