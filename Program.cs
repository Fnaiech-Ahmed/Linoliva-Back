using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Text;
using System.Security.Claims;
using System.Threading.Tasks;
using tech_software_engineer_consultant_int_backend.Models;
using tech_software_engineer_consultant_int_backend.Repositories;
using tech_software_engineer_consultant_int_backend.Services;
using tech_software_engineer_consultant_int_backend.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using SignalRWebpack.Hubs;
using System.Linq;
using System.Data;
using Microsoft.AspNetCore.DataProtection.Repositories;
using System.Configuration;
using tech_software_engineer_consultant_int_backend.Middleware;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Community;

// Configuration de la base de données
/*var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(connectionString));*/
var connectionString = builder.Configuration.GetConnectionString("Default");

// Ajouter Encrypt=False ou TrustServerCertificate=True si SSL pose problème
builder.Services.AddDbContext<MyDbContext>(options =>
{
    options.UseSqlServer(
        connectionString,
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(); // réessais automatique en cas d'erreurs transitoires
        }
    );
});

// Ajouter les services Identity
builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<MyDbContext>()
    .AddDefaultTokenProviders();

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy => policy.WithOrigins("https://linoliva.netlify.app")
                        //.WithOrigins("http://localhost:3000") // Replace with your frontend URL
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
});


// Récupération des informations Twilio depuis la configuration
var twilioSettings = builder.Configuration.GetSection("Twilio");
var accountSid = twilioSettings.GetValue<string>("AccountSid");
var authToken = twilioSettings.GetValue<string>("AuthToken");
var fromNumber = twilioSettings.GetValue<string>("FromNumber");

// Enregistrement du service SMS
//builder.Services.AddSingleton(new SmsService(accountSid, authToken, fromNumber));


// Enregistrement des contrôleurs
builder.Services.AddControllers(); // Nécessaire pour les contrôleurs API

// Ajouter les services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenAuthenticationService, TokenAuthenticationService>();
builder.Services.AddScoped<ISmsService, SmsService>(provider =>
{
    return new SmsService(accountSid, authToken, fromNumber);
});
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IRapportService, RapportService>();
builder.Services.AddScoped<IPortefeuilleService, PortefeuilleService>();
builder.Services.AddScoped<IImpayeService, ImpayeService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IDossierService, DossierService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IUserPortefeuilleAssociationService, UserPortefeuilleAssociationService>();
builder.Services.AddScoped<IEcheanceService, EcheanceService>();
builder.Services.AddScoped<IAbonnementService, AbonnementService>();
builder.Services.AddScoped<ILicenceService, LicenceService>();
// Services métiers
builder.Services.AddScoped<IActivationService, ActivationService>();


builder.Services.AddScoped<ISIService, SIService>();
builder.Services.AddScoped<IInventaireProduitService, InventaireProduitService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ILotService, LotService>();
builder.Services.AddScoped<ILotsTransactionsService, LotsTransactionsService>();
builder.Services.AddScoped<ICommandeService, CommandeService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IRapportJournalierService, RapportJournalierService>();

builder.Services.AddScoped<IBLService, BLService>();
builder.Services.AddScoped<IFactureService, FactureService>();
builder.Services.AddScoped<IBonDeSortieService, BonDeSortieService>();

builder.Services.AddScoped<IUserRepository<User>, UserRepository>();
builder.Services.AddScoped<IRoleRepository<Role>, RoleRepository>();
builder.Services.AddScoped<IPortefeuilleRepository<Portefeuille>, PortefeuilleRepository>();
builder.Services.AddScoped<IRapportRepository<Rapport>, RapportRepository>();
builder.Services.AddScoped<ISequenceRepository<Sequence>, SequenceRepository>();
builder.Services.AddScoped<IUserPortefeuilleAssociationRepository<UserPortefeuilleAssociation>, UserPortefeuilleAssociationRepository>();
builder.Services.AddScoped<IEcheanceRepository, EcheanceRepository>();
builder.Services.AddScoped<IImpayeRepository, ImpayeRepository>();
builder.Services.AddScoped<IAbonnementRepository, AbonnementRepository>();
builder.Services.AddScoped<ILicenceRepository, LicenceRepository>();

builder.Services.AddScoped<ISIRepository<SI>, SIRepository>();
builder.Services.AddScoped<IProductRepository<Product>, ProductRepository>();
builder.Services.AddScoped<ILotRepository, LotRepository>();
builder.Services.AddScoped<IInventaireProduitRepository, InventaireProduitRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ICommandeRepository, CommandeRepository>();
builder.Services.AddScoped<ILotsTransactionsRepository, LotsTransactionsRepository>();
builder.Services.AddScoped<IBLRepository, BLRepository>();
builder.Services.AddScoped<IFactureRepository, FactureRepository>();
builder.Services.AddScoped<IBonDeSortieRepository, BonDeSortieRepository>();
builder.Services.AddScoped<IRapportJournalierRepository, RapportJournalierRepository>();


// Ajouter les nouveaux services et repositories pour UserRole
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();

// Ajouter les nouveaux services et repositories pour RolePolicyRoles
builder.Services.AddScoped<IRolePolicyRolesRepository, RolePolicyRolesRepository>();
builder.Services.AddScoped<IRolePolicyRolesService, RolePolicyRolesService>();

builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddScoped<IPolicyLoader, PolicyLoader>();


builder.Services.AddAutoMapper(typeof(Program), typeof(MappingProfile));

// Configuration de Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Ajouter le service SignalR
builder.Services.AddSignalR();


// Configuration de l'authentification basée sur les jetons
builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Audience = "resource-server";
    options.Authority = builder.Configuration["frontEndUrl"];
    options.RequireHttpsMetadata = false;
    options.IncludeErrorDetails = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = ClaimTypes.Name, // Assurez-vous que cela correspond à la revendication de nom dans le jeton
        RoleClaimType = "role" // Assurez-vous que cela correspond à la revendication de rôle dans le jeton
    };
});


// Ajoutez les services d'autorisation
builder.Services.AddAuthorization();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // Initialiser les données de seed
    var userManager = services.GetRequiredService<UserManager<User>>();
    var roleManager = services.GetRequiredService<RoleManager<Role>>();
    var userService = services.GetRequiredService<IUserService>();
    await SeedData.Initialize(services, userManager, roleManager, userService);


    var rolePolicyRolesService = scope.ServiceProvider.GetRequiredService<IRolePolicyRolesService>();
    var rolePolicyRoles = await rolePolicyRolesService.GetAllRolePolicyRolesAsync();

    // Accédez au service d'options d'autorisation
    var options = app.Services.GetRequiredService<IOptions<AuthorizationOptions>>().Value;

    // Ajoutez les politiques
    foreach (var rolePolicyRole in rolePolicyRoles)
    {
        var policyName = rolePolicyRole.RolePolicy?.PolicyName ?? "DefaultPolicyName";
        var roleName = rolePolicyRole.Role?.Name ?? "DefaultRoleName";

        options.AddPolicy(policyName, policy =>
        {
            policy.RequireRole(roleName);
            Console.WriteLine($"Policy Name: {policyName}, Roles: {string.Join(", ", roleName)}");
        });
    }

}


// Utiliser la politique CORS
app.UseCors("AllowSpecificOrigin");


// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting(); // Active le middleware de routage
app.UseAuthentication(); // Middleware d'authentification
app.UseAuthorization();  // Middleware d'autorisation

// après UseAuthentication() et UseAuthorization()
// ton middleware de licence/abonnement passe ici
app.UseMiddleware<ActivationMiddleware>();


app.MapHub<ChatHub>("/hub");
app.MapControllers(); // Assurez-vous que les contrôleurs sont mappés

app.Run();
