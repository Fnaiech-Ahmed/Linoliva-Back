namespace tech_software_engineer_consultant_int_backend.DTO
{
    // DTOs/ValidateActivationResultDTO.cs
    public class ValidateActivationResultDTO
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? AccessType { get; set; } // "Abonnement" | "Licence" | null
        public DateTime? ExpirationDate { get; set; }
    }
}
