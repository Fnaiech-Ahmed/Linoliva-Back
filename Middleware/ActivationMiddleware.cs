using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using tech_software_engineer_consultant_int_backend.Services;

namespace tech_software_engineer_consultant_int_backend.Middleware
{
    public class ActivationMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly string[] ExcludedPaths = new[]
        {
            "/api/auth",
            "/api/Activation/validate",
            "/api/Activation/register-device",
            "/api/Activation/create",
            "/api/Activation/assign",
            "/swagger",
            "/health",
            "/api/Product",
            "/api/Product/add-product",
            "/api/Product/get-list-products",
            "/api/Product/externe-get-list-products",
            "/api/RapportJournalier/**",
            "/api/Commande/Get-List-Commandes",
            "/api/Commande/get-Commande-ByID/",
            "/api/Commande",
            "/api/Commande/Create-Commande",
            "/api/Transaction",
            "/api/Facture",
            "/api/InventaireProduit",
            "/api/Lot",
            "/api/Lot/add-Lot",
            "/api/Lot/Delete-Lot",

            "/api/Transaction/GetTransactions",
            "/api/Transaction/getTransactionbyRef/**",
            "/api/Transaction/getTransactionbyRef/**",
            "/api/Transaction/DeleteTransaction/**",
            "/api/Transaction/getTransactionbyRef/**",


            "/api/Lot/**",
            "/api/Facture/Create-Facture",
            "/api/Facture/Get-List-Factures",
            "/api/Facture/get-Facture-ID/{id}",
            "/api/Facture/update-Facture/{id}",

            "/api/BonDeSortie/Create-Bon-De-Sortie",
            "/api/BonDeSortie/Get-List-Bons-De-Sortie",
            "/api/BonDeSortie/get-Bon-De-Sortie-ByID/{id}",
            "/api/BonDeSortie/get-Bon-De-Sortie-ByRef/{reference}",
            "/api/Lot/get-List-Lots",
            "/api/User",
            "/api/User/create-user",
            "/api/User/addUser",
            "/api/User/GetUsers",
            "/api/User/getUser/{id}",
            "/api/User/updateUser/{id}",
            "/api/User/deleteUser/{id}",
            "/api/User/forgot-password",
            "/api/User/reset-password",
            "/api/BonDeLivraison",
            "/api/Role",
            "/api/Role/Create-Role",
            "/api/Role/get-list-Roles",
            "/api/Role/Update-Role/{id}",
            "/api/Role/Delete-Role/{id}",

        };

        public ActivationMiddleware(RequestDelegate next) { _next = next; }

        public async Task Invoke(HttpContext context, IActivationService activationService)
        {
            var path = context.Request.Path.Value ?? string.Empty;
            if (ExcludedPaths.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
            {
                await _next(context);
                return;
            }

            // Prefer claims if authenticated
            var email = context.User.FindFirst(ClaimTypes.Email)?.Value;
            var phone = context.User.FindFirst("phone")?.Value ?? context.User.FindFirst("mobile")?.Value;

            // Also check headers (for MAUI copy)
            if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(phone))
            {
                email = context.Request.Headers["X-User-Email"].FirstOrDefault();
                phone = context.Request.Headers["X-User-Phone"].FirstOrDefault();
            }

            var activationCode = context.Request.Headers["X-Activation-Code"].FirstOrDefault();
            var deviceId = context.Request.Headers["X-Device-Id"].FirstOrDefault();

            // If activation code is present, validate that code+device are ok (preferred)
            if (!string.IsNullOrWhiteSpace(activationCode))
            {
                var validation = await activationService.ValidateActivationAsync(activationCode, deviceId, email, phone);
                if (!validation.Success)
                {
                    context.Response.StatusCode = StatusCodes.Status402PaymentRequired;
                    await context.Response.WriteAsync(validation.Message);
                    return;
                }

                // inject claim if not present
                var identity = context.User.Identities.FirstOrDefault();
                if (identity != null && !identity.HasClaim(c => c.Type == "access"))
                {
                    identity.AddClaim(new Claim("access", validation.AccessType ?? "Subscription"));
                }
                await _next(context);
                return;
            }

            // otherwise if user authenticated, check assigned codes by email/phone
            if (!string.IsNullOrWhiteSpace(email) || !string.IsNullOrWhiteSpace(phone))
            {
                var access = await activationService.GetAccessForUserAsync(email, phone);
                if (!access.IsValid)
                {
                    context.Response.StatusCode = StatusCodes.Status402PaymentRequired;
                    await context.Response.WriteAsync(access.Message);
                    return;
                }

                var identity = context.User.Identities.FirstOrDefault();
                if (identity != null && !identity.HasClaim(c => c.Type == "access"))
                {
                    identity.AddClaim(new Claim("access", access.AccessType ?? "Subscription"));
                }

                await _next(context);
                return;
            }

            // No activation info -> block or allow depending on your policy
            // Here we block (app must authenticate or supply activation headers)
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Authentification ou activation requise.");
        }
    }

    // Extension to register middleware easily
    public static class ActivationMiddlewareExtensions
    {
        public static IApplicationBuilder UseActivationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ActivationMiddleware>();
        }
    }
}
