using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public class PolicyLoader : IPolicyLoader
    {
        private readonly IServiceProvider _serviceProvider;

        public PolicyLoader(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task LoadPolicies(AuthorizationOptions options)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();
                var rolePolicies = dbContext.RolePolicies.ToList();
                //var rolePolicyRoles = dbContext.RolePolicyRoles.Include(rpr => rpr.RolePolicy).ToList();
                var rolePolicyRoles = await dbContext.RolePolicyRoles.Include(rpr => rpr.Role).ToListAsync();

                var policyRoles = rolePolicyRoles
                    .GroupBy(rpr => rpr.RolePolicy.PolicyName)
                    .ToDictionary(
                    g => g.Key,
                    g => g.Select(rpr => rpr.Role.Name).ToArray()
                    );


                foreach (var policy in rolePolicies)
                {
                    if (policyRoles.TryGetValue(policy.PolicyName, out var roles))
                    {
                        options.AddPolicy(policy.PolicyName, policyBuilder =>
                            policyBuilder.RequireRole(roles));

                        Console.WriteLine($"Policy added: {policy.PolicyName} with roles: {string.Join(", ", roles)}");
                    }
                }
            }
        }
    }

}
