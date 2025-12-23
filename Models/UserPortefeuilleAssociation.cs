namespace tech_software_engineer_consultant_int_backend.Models
{
    public class UserPortefeuilleAssociation
    {
        public int Id { get; set; }

        public int? UserId { get; set; } // Clé étrangère vers User
        public User? User { get; set; }

        public int? PortefeuilleId { get; set; } // Clé étrangère vers PortefeuilleId
        public Portefeuille? Portefeuille { get; set; }

    }
}