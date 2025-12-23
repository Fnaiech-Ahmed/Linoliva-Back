namespace tech_software_engineer_consultant_int_backend.DTO.ProductDTOs
{
    public class ProductYearKey
    {
        public string ProductName { get; set; }
        public List<Dictionary<int, decimal>> YearsMontants { get; set; }
    }
}
