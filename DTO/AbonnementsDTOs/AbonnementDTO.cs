namespace tech_software_engineer_consultant_int_backend.DTO
{
    public class AbonnementDTO
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime? DateFin { get; set; }

        // Informations du produit
        

        //public int? DossierId { get; set; }
    }
}
