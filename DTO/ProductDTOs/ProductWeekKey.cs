namespace tech_software_engineer_consultant_int_backend.DTO.ProductDTOs
{
    public class ProductWeekKey
    {
        public string ProductName { get; set; }
        public List<Dictionary<string, decimal>> WeeksMontants { get; set; } // Clé de type string pour la semaine

    }
}
