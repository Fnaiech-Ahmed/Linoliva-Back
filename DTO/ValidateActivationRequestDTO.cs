namespace tech_software_engineer_consultant_int_backend.DTO
{

    // DTOs/ValidateActivationRequestDTO.cs
    public class ValidateActivationRequestDTO
    {
        public string ActivationCode { get; set; } = null!;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? DeviceId { get; set; }
    }
}
