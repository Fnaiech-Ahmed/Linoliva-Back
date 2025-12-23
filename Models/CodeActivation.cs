namespace tech_software_engineer_consultant_int_backend.Models
{
    public class CodeActivation
    {
        public int Id { get; set; }
        public string ActivationCodeReference { get; set; } // Code commun pour Abonnement et Licence
        public DateTime? ActivationDate { get; set; } // Date de début de Code d'activation
        public DateTime? ExpirationDate { get; set; } // Date de fin de Code d'activation
        public string ActivationCode { get; set; }
        public string BusinessSector {  get; set; } // Secteur d'Activité 


        // Informations du produit
        public int ProductId { get; set; } // A ne pas toucher, valeur affectée lors de la première création du code d'activation
        public string? ProductReference { get; set; } // Référence du produit auquel le code est affecté
        public string? ClientRef { get; set; } // Identifiant du client

        public string? AssignedToEmail { get; set; }
        public string? AssignedToPhone { get; set; }
        public bool IsAssigned => !string.IsNullOrEmpty(AssignedToEmail) || !string.IsNullOrEmpty(AssignedToPhone);


        public int MaxUsersAllowed { get; set; } // Nombre maximum d'utilisateurs autorisés

        public int MaxDevicesAllowed { get; set; } // Nombre maximum d'appareils autorisés


        public List<ActivationDevice> ActivationDevices { get; set; } = new();

    }

}
