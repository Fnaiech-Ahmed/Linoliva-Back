using System.ComponentModel.DataAnnotations;
using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Models
{
    public class Abonnement : CodeActivation
    {
        public string SubscriptionType { get; set; } // Type d'abonnement (Mensuel, Annuel, etc.)
        public bool IsRecurring { get; set; } // Indique si l'abonnement est récurrent (renouvelable automatiquement)
        public bool IsActive { get; set; } // Statut actif ou non
        public string PaymentMethod { get; set; } // Méthode de paiement
        public DateTime? RenewalDate { get; set; } // Date de renouvellement de l'abonnement
        public int RenewalAttempts { get; set; } // Nombre de tentatives de renouvellement
        public DateTime? CancelledDate { get; set; } // Date d'annulation (si applicable)

        /*public int? DossierId { get; set; }
         * public Dossier? Dossier { get; set; }*/

        public int? UserId { get; set; }
        public User? User { get; set; }



        // -------- CONSTRUCTEUR PAR DÉFAUT --------
        public Abonnement()
        {
            ActivationDevices = new List<ActivationDevice>();
        }

        // -------- CONSTRUCTEUR DE COPIE --------
        public Abonnement(CodeActivation source)
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
        }



    }

}
