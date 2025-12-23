namespace tech_software_engineer_consultant_int_backend.DTO.ProductDTOs
{
    public class ProductQYearKey
    {
        public string ProductName { get; set; }
        public List<Dictionary<int, decimal>> YearsQuantites { get; set; }
    }
}
