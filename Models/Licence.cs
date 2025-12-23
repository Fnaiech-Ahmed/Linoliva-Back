namespace tech_software_engineer_consultant_int_backend.Models
{
    public class Licence : CodeActivation
    {
        public string LicenceType { get; set; } // Type de licence (Standard, Premium, etc.)
        public void SetAsLifetime()
        {
            ExpirationDate = DateTime.MaxValue; // Définit la licence comme à vie
        }
        public bool IsActive { get; set; } // Statut actif ou non
        public int ActivationAttempts { get; set; } // Nombre de tentatives d'activation
        public string ActivatedBy { get; set; } // Qui a activé la licence
        public string DeviceId { get; set; } // Identifiant de l'appareil
        public string Region { get; set; } // Région dans laquelle la licence est valide

        public int? UserId { get; set; }
        public User? User { get; set; }

        // -------- CONSTRUCTEUR DE COPIE --------
        public Licence(CodeActivation source)
        {
            Id = source.Id;
            ActivationCode = source.ActivationCode;
            ActivationCodeReference = source.ActivationCodeReference;
            ActivationDate = source.ActivationDate;
            ExpirationDate = source.ExpirationDate;
            ProductId = source.ProductId;
            ProductReference = source.ProductReference;
            ClientRef = source.ClientRef;
            AssignedToEmail = source.AssignedToEmail;
            AssignedToPhone = source.AssignedToPhone;
            BusinessSector = source.BusinessSector;
            MaxUsersAllowed = source.MaxUsersAllowed;
            MaxDevicesAllowed = source.MaxDevicesAllowed;
            ActivationDevices = new List<ActivationDevice>(source.ActivationDevices);

            // spécifique
            //IsLifetime = source.IsLifetime;
        }

        public Licence() { }

    }

}
