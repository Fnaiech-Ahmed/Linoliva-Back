namespace tech_software_engineer_consultant_int_backend.DTO.ProductDTOs
{
    public class ProductDayKey
    {
        public string ProductName { get; set; }
        public Dictionary<string, decimal> DaysMontants { get; set; } = new();
    }

}
