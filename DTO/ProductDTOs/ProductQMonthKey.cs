namespace tech_software_engineer_consultant_int_backend.DTO.ProductDTOs
{
    public class ProductQMonthKey
    {
        public string ProductName { get; set; }
        public List<Dictionary<string, decimal>> MonthsQuantites { get; set; }
    }
}
