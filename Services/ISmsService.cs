namespace tech_software_engineer_consultant_int_backend.Services
{
    public interface ISmsService
    {
        Task SendSmsAsync(string toNumber, string message);
    }
}
