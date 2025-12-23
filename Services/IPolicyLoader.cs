using Microsoft.AspNetCore.Authorization;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public interface IPolicyLoader
    {
        Task LoadPolicies(AuthorizationOptions options);
    }
}
