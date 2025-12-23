using tech_software_engineer_consultant_int_backend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public class TokenAuthenticationService : ITokenAuthenticationService
    {
        private readonly IConfiguration _configuration;
        private readonly MyDbContext _context;
        private readonly IRoleService _roleService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TokenAuthenticationService(IConfiguration configuration, MyDbContext context, IRoleService roleService, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _context = context;
            _roleService = roleService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> GenerateTokenAsync(string userId, string username, Role role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("MaCleSecrete12345678901234567890");

            // Obtenir les politiques du rôle
            var policies = await _roleService.GetPoliciesForRoleAsync(role.Id);

            // Créer les revendications du jeton
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim("UserId", userId),
                new Claim(ClaimTypes.Role, role != null ? role.Name : "user")
            };

            // Ajouter les revendications de politique
            foreach (var policy in policies)
            {
                claims.Add(new Claim("policy", policy));
            }

            // Créer le descripteur de jeton
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var encodedToken = tokenHandler.WriteToken(token);

            // Enregistrer le jeton dans la base de données
            SaveTokenInDatabase(username, encodedToken);

            return encodedToken;
        }




        private void SaveTokenInDatabase(string username, string token)
        {
            // Ici la logique pour enregistrer le jeton dans la base de données
            // Utilisons le contexte (_context) pour accéder à la base de données et enregistrer le jeton associé à l'utilisateur
            var user = _context.Users.FirstOrDefault(u => u.Email == username);
            
            if (user != null)
            {
                user.Token = token;
                _context.SaveChanges();
            }
            else
            {
                // Gérer le cas où l'utilisateur n'est pas trouvé
                // Par exemple, journaliser un avertissement ou effectuer une autre action appropriée
                throw new InvalidOperationException("Utilisateur non trouvé dans la base de données.");
            }
        }



        public bool ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("MaCleSecrete12345678901234567890");
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                }, out _);
                return true;
            }
            catch
            {
                return false;
            }
        }


        public bool ValidateCredentials(LoginRequest loginRequest)
        {
            //User? existingUser = _context.Users.FirstOrDefault(u => u.Email == model.Username && u.PasswordHash == model.Password);
            User? existingUser = _context.Users.FirstOrDefault(u => u.Email == loginRequest.Username);

            if (existingUser != null)
            {
                var passwordHasher = new PasswordHasher<object>();

                string plainPassword = loginRequest.Password;

                Console.WriteLine($"Plain Password: {plainPassword}");

                var verificationResult = passwordHasher.VerifyHashedPassword(null, existingUser.PasswordHash, plainPassword);

                if (verificationResult == PasswordVerificationResult.Success)
                {
                    Console.WriteLine("Password matches.");

                    // Convertir l'ID de l'utilisateur en string si nécessaire
                    var userIdString = existingUser.Id.ToString();
                    return true;
                }
                else
                {
                    Console.WriteLine("Password does not match.");
                    return false;
                }

            }
            else
            {
                return false;
            }
        }


        public string GetUserId(LoginRequest model)
        {
            //User? existingUser = _context.Users.FirstOrDefault(u => u.Email == model.Username && u.PasswordHash == model.Password);
            User? existingUser = _context.Users.FirstOrDefault(u => u.Email == model.Username);

            if (existingUser != null)
            {
                var passwordHasher = new PasswordHasher<object>();

                string plainPassword = model.Password;
                
                Console.WriteLine($"Plain Password: {plainPassword}");                

                var verificationResult = passwordHasher.VerifyHashedPassword(null, existingUser.PasswordHash, plainPassword);

                if (verificationResult == PasswordVerificationResult.Success)
                {
                    Console.WriteLine("Password matches.");

                    // Convertir l'ID de l'utilisateur en string si nécessaire
                    var userIdString = existingUser.Id.ToString();
                    return userIdString;
                }
                else
                {
                    Console.WriteLine("Password does not match.");
                    return null;
                }
                                
            }
            else
            {
                return null;
            }
        }


        public string GetCurrentUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }



        public Role GetUserRole(LoginRequest model)
        {
            //User? existingUser = _context.Users.FirstOrDefault(u => u.Email == model.Username && u.Mp == model.Password);
            User? existingUser = _context.Users.FirstOrDefault(u => u.Email == model.Username);

            if (existingUser != null && existingUser.RoleId != null)
            {
                Role? existingRole = _context.Roles.FirstOrDefault(r => r.Id == existingUser.RoleId);
                if (existingRole != null)
                {
                    return existingRole;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }


        public Role GetUserRoleById(int userId)
        {
            User? existingUser = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (existingUser != null)
            {
                Role? existingRole = _context.Roles.FirstOrDefault(r => r.Id == existingUser.RoleId);
                if (existingRole != null)
                {
                    return existingRole;
                }
                else
                {
                    return null;
                }

            }
            else
            {
                return null;
            }
        }



        public ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            var identity = new ClaimsIdentity(jwtToken.Claims, JwtBearerDefaults.AuthenticationScheme);

            return new ClaimsPrincipal(identity);
        }


        public string GetUserIdFromToken(HttpContext httpContext)
        {
            var token = httpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (!string.IsNullOrEmpty(token))
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);

                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId");
                if (userIdClaim != null)
                {
                    return userIdClaim.Value;
                }
            }

            return null;
        }



        public bool IsUserAuthorized(string token, string requiredPolicy)
        {
            bool isValidToken = ValidateToken(token);
            if (!isValidToken) return false;

            ClaimsPrincipal user = GetPrincipalFromToken(token);
            var userPolicies = user.FindAll("policy").Select(p => p.Value);
            return userPolicies.Contains(requiredPolicy);
        }



    }
}