namespace tech_software_engineer_consultant_int_backend.DTO.ProductDTOs
{
    public class ProductMonthKey
    {
        public string ProductName { get; set; }
        public List<Dictionary<string, decimal>> MonthsMontants { get; set; }
    }
}
