namespace tech_software_engineer_consultant_int_backend.Models
{
    public class VenteResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<(Lot Lot, int QuantitePrelevee)> LotsImpactes { get; set; } = new();
    }
}
