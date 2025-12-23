using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace tech_software_engineer_consultant_int_backend.Models
{
    public class User : IdentityUser<int>
    {
        public string Ref { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public string? Mp { get; set; }
        public bool IsEnabled { get; set; } = false;

        public int? RoleId { get; set; }
        public Role? Role { get; set; }
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        public string? Action { get; set; }
        public string? Civilite { get; set; }
        public string? Adresse { get; set; }
        public string? Pays { get; set; }
        public string? Ville { get; set; }
        public string? CodePostal { get; set; }
        public string? Status { get; set; }

        public string? Token { get; set; }

        // Prestataire
        public string? Fax { get; set; }
        public string? Agence { get; set; }
        public string? DossierEncharge { get; set; }

        // Recouvreur
        public string? Etat { get; set; }
        public string? Montant { get; set; }
        public int? NombreDossier { get; set; }
        public string? Fonction { get; set; }
        public string? Grade { get; set; }
        public int? Anciennete { get; set; }

        // Client
        public string? Segmentation { get; set; }
        public string? Nationalité { get; set; }
        public DateTime? DateDeNaissance { get; set; }
        public string? SituationFamiliale { get; set; }

        public List<Notification>? ListeNotifications { get; set; } = new List<Notification>();
        public List<UserPortefeuilleAssociation>? ListeUserPortefeuilles { get; set; } = new List<UserPortefeuilleAssociation>();
        public List<UserGroupAssociation>? UserGroupAssociations { get; set; } = new List<UserGroupAssociation>();
        public List<Message>? MessagesSent { get; set; } = new List<Message>();



        public ICollection<Abonnement>? Abonnements { get; set; }
        public ICollection<Licence>? Licences { get; set; }

        public bool HasActiveSubscription =>
            Abonnements?.Any(
                a =>
                a.IsActive &&
                a.ExpirationDate >= DateTime.UtcNow) == true;

        public bool HasActiveLicence =>
            Licences?.Any(
                l =>
                l.IsActive &&
                l.ExpirationDate >= DateTime.UtcNow) == true;

        public bool HasAnyValidAccess =>
            HasActiveSubscription || HasActiveLicence;




        // Constructeur par défaut
        public User() { }

        // Constructeur avec paramètres pour les propriétés obligatoires
        public User(string reference, string firstName, string lastName, string mobile, string email)
        {
            Ref = reference;
            FirstName = firstName;
            LastName = lastName;
            Mobile = mobile;
            Email = email;
        }

        // Constructeur du Prestataire
        public User(string reference, string civilite, string adresse, string pays, string ville, string codePostal, string agence, string status, string firstName, string lastName, string mobile, string email)
            : this(reference, firstName, lastName, mobile, email)
        {
            Civilite = civilite;
            Adresse = adresse;
            Pays = pays;
            Ville = ville;
            CodePostal = codePostal;
            Agence = agence;
            Status = status;
        }

        // Constructeur du Recouvreur
        /*public User(string reference, string firstName, string lastName, string mobile, string email, string civilite, string adresse, string pays, string ville, string codePostal, string fonction, string grade, int anciennete, string status)
            : this(reference, firstName, lastName, mobile, email, civilite, adresse, pays, ville, codePostal, status)
        {
            Fonction = fonction;
            Grade = grade;
            Anciennete = anciennete;
        }*/

        // Constructeur Pour UserCreateDto
        public User(string firstName, string lastName, string mobile, string email)
        {
            FirstName = firstName;
            LastName = lastName;
            Mobile = mobile;
            Email = email;
        }
    }
}
