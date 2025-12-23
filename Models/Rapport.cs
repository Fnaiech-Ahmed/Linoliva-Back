namespace tech_software_engineer_consultant_int_backend.Models
{
    public class Rapport
    {
        public int Id { get; set; }
        public int? PortefeuilleId { get; set; }
        public Portefeuille? Portefeuille { get; set;}
    }
}
