using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using tech_software_engineer_consultant_int_backend.Models;
using tech_software_engineer_consultant_int_backend.Services;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider, UserManager<User> userManager, RoleManager<Role> roleManager, IUserService userService)
    {
        using (var context = new MyDbContext(serviceProvider.GetRequiredService<DbContextOptions<MyDbContext>>()))
        {
            context.Database.EnsureCreated();

            // Création des rôles
            if (!context.Roles.Any())
            {
                var roles = new[]
                {
                    new Role
                    {
                        Name = "admin",
                        NormalizedName = "ADMIN",
                        Description = "Administrator role",
                        ConcurrencyStamp = Guid.NewGuid().ToString()
                    },
                    new Role
                    {
                        Name = "user",
                        NormalizedName = "USER",
                        Description = "Regular user role",
                        ConcurrencyStamp = Guid.NewGuid().ToString()
                    }
                };

                foreach (var role in roles)
                {
                    await roleManager.CreateAsync(role);
                }
            }

            // Création des utilisateurs
            if (!context.Users.Any())
            {
                var adminRole = context.Roles.FirstOrDefault(r => r.Name == "admin");
                var userRole = context.Roles.FirstOrDefault(r => r.Name == "user");

                if (adminRole == null || userRole == null)
                {
                    throw new InvalidOperationException("Roles doivent être créés avant d'ajouter des utilisateurs.");
                }

                var users = new[]
                {
                    new User
                    {
                        UserName = "ahmed",
                        NormalizedUserName = "AHMED",
                        Email = "fnaiech.ahmed1@gmail.com",
                        NormalizedEmail = "fnaiech.ahmed1@gmail.com",
                        EmailConfirmed = true,
                        PasswordHash = new PasswordHasher<User>().HashPassword(null, "Admin@123"),
                        SecurityStamp = Guid.NewGuid().ToString(),
                        FirstName = "Ahmed",
                        LastName = "Fn",
                        RoleId = adminRole.Id,
                        Mobile = "52172703",
                        IsEnabled = true,
                        /*Ref = userService.GenerateNextRef() // Utilisation de la méthode pour générer Ref*/
                        Ref = await userService.GenerateNextRef()
                    },

                    new User
                    {
                        UserName = "user",
                        NormalizedUserName = "USER",
                        Email = "user@example.com",
                        NormalizedEmail = "USER@EXAMPLE.COM",
                        EmailConfirmed = true,
                        PasswordHash = new PasswordHasher<User>().HashPassword(null, "User@123"),
                        SecurityStamp = Guid.NewGuid().ToString(),
                        FirstName = "Regular",
                        LastName = "User",
                        RoleId = userRole.Id,
                        Mobile = "52172703",
                        IsEnabled = true,
                        /*Ref = userService.GenerateNextRef() // Utilisation de la méthode pour générer Ref*/
                        Ref = await userService.GenerateNextRef()
                    }
                };

                foreach (var user in users)
                {
                    await userManager.CreateAsync(user);
                }
            }

            // Ajouter les UserRoles
            if (!context.UserRoles.Any())
            {
                var adminUser = await userManager.FindByNameAsync("ahmed");
                var regularUser = await userManager.FindByNameAsync("user");

                var adminRole = await roleManager.FindByNameAsync("admin");
                var userRole = await roleManager.FindByNameAsync("user");

                var userRoles = new[]
                {
                    new UserRole
                    {
                        UserId = adminUser.Id,
                        RoleId = adminRole.Id
                    },
                    new UserRole
                    {
                        UserId = regularUser.Id,
                        RoleId = userRole.Id
                    }
                };

                context.UserRoles.AddRange(userRoles);
                await context.SaveChangesAsync();
            }

            // Ajouter les politiques dans la base de données
            if (!context.RolePolicies.Any())
            {
                var policies = new[]
                {
                    new RolePolicy { PolicyName = "UserPolicy" },
                    new RolePolicy { PolicyName = "AdminPolicy" },
                    new RolePolicy { PolicyName = "AbonnementPolicy" },
                    new RolePolicy { PolicyName = "AbonnementSeniorPolicy" },
                    new RolePolicy { PolicyName = "BonDeLivraisonPolicy" },
                    new RolePolicy { PolicyName = "BonDeLivraisonSeniorPolicy" },
                    new RolePolicy { PolicyName = "BonDeSortiePolicy" },
                    new RolePolicy { PolicyName = "BonDeSortieSeniorPolicy" },
                    new RolePolicy { PolicyName = "CommandePolicy" },
                    new RolePolicy { PolicyName = "CommandeSeniorPolicy" },
                    new RolePolicy { PolicyName = "CompteBancairePolicy" },
                    new RolePolicy { PolicyName = "CompteBancaireSeniorPolicy" },
                    new RolePolicy { PolicyName = "DossierPolicy" },
                    new RolePolicy { PolicyName = "DossierSeniorPolicy" },
                    new RolePolicy { PolicyName = "EcheancePolicy" },
                    new RolePolicy { PolicyName = "EcheanceSeniorPolicy" },
                    new RolePolicy { PolicyName = "FacturePolicy" },
                    new RolePolicy { PolicyName = "FactureSeniorPolicy" },
                    new RolePolicy { PolicyName = "ImpayePolicy" },
                    new RolePolicy { PolicyName = "ImpayeSeniorPolicy" },
                    new RolePolicy { PolicyName = "InventaireProduitPolicy" },
                    new RolePolicy { PolicyName = "InventaireProduitSeniorPolicy" },
                    new RolePolicy { PolicyName = "LotPolicy" },
                    new RolePolicy { PolicyName = "LotSeniorPolicy" },
                    new RolePolicy { PolicyName = "NotificationPolicy" },
                    new RolePolicy { PolicyName = "NotificationSeniorPolicy" },
                    new RolePolicy { PolicyName = "PortefeuillePolicy" },
                    new RolePolicy { PolicyName = "PortefeuilleSeniorPolicy" },
                    new RolePolicy { PolicyName = "ProductPolicy" },
                    new RolePolicy { PolicyName = "ProductSeniorPolicy" },
                    new RolePolicy { PolicyName = "RapportPolicy" },
                    new RolePolicy { PolicyName = "RapportSeniorPolicy" },
                    new RolePolicy { PolicyName = "RolesPolicies" },
                    new RolePolicy { PolicyName = "RolesSeniorPolicies" }
                };

                context.RolePolicies.AddRange(policies);
                await context.SaveChangesAsync();
            }

            // Ajouter les RolePolicyRoles
            if (!context.RolePolicyRoles.Any())
            {
                var adminRole = await roleManager.FindByNameAsync("admin");
                var userRole = await roleManager.FindByNameAsync("user");


                //var adminPolicy = context.RolePolicies.First(p => p.PolicyName == "AdminPolicy");
                //var userPolicy = context.RolePolicies.First(p => p.PolicyName == "UserPolicy");

                /*var rolePolicyRoles = new[]
                {
                    new RolePolicyRole
                    {
                        RoleId = adminRole.Id,
                        RolePolicyId = adminPolicy.Id
                    },
                    new RolePolicyRole
                    {
                        RoleId = userRole.Id,
                        RolePolicyId = userPolicy.Id
                    }
                };*/



                var adminPolicies = context.RolePolicies.Where(p => p.PolicyName.EndsWith("Policy")).ToList();
                var userPolicies = context.RolePolicies.Where(p => p.PolicyName == "UserPolicy").ToList();               

                

                var rolePolicyRoles = new List<RolePolicyRole>();

                // Associer toutes les politiques au rôle admin
                foreach (var policy in adminPolicies)
                {
                    rolePolicyRoles.Add(new RolePolicyRole
                    {
                        RoleId = adminRole.Id,
                        RolePolicyId = policy.Id
                    });
                }

                // Associer la politique UserPolicy au rôle user
                foreach (var policy in userPolicies)
                {
                    rolePolicyRoles.Add(new RolePolicyRole
                    {
                        RoleId = userRole.Id,
                        RolePolicyId = policy.Id
                    });
                }

                context.RolePolicyRoles.AddRange(rolePolicyRoles);
                await context.SaveChangesAsync();
            }
        }
    }
}
