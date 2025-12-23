
using tech_software_engineer_consultant_int_backend.Models;
using System.Security.Claims;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public interface ITokenAuthenticationService
    {
        //public int GeneratePinCode(int length = 4);
        //Task<int> GenerateAndSavePinCodeAsync(string PhoneNumber);
        Task<string> GenerateTokenAsync(string userId, string username, Role role);
        public string GetUserId(LoginRequest loginRequest);
        //public string GetUserId2(LoginPhoneRequest loginRequest);
        public string GetCurrentUserId();
        public Role GetUserRole(LoginRequest model);
        //public Role GetUserRole2(LoginPhoneRequest model);
        public Role GetUserRoleById(int userId);
        //public string GetUserUserName(LoginPhoneRequest model);
        bool ValidateToken(string token);
        bool ValidateCredentials(LoginRequest loginRequest);
        //bool ValidateCredentialsByPhone(LoginPhoneRequest loginPhoneRequest);
        ClaimsPrincipal GetPrincipalFromToken(string token);
        string GetUserIdFromToken(HttpContext httpContext);


        bool IsUserAuthorized(string token, string requiredPolicy);
    }
}
