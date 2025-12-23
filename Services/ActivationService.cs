using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using tech_software_engineer_consultant_int_backend.DTO;
using tech_software_engineer_consultant_int_backend.DTO.ActivationCodesDTOs;
using tech_software_engineer_consultant_int_backend.DTO.ActivationDTOs;
using tech_software_engineer_consultant_int_backend.DTO.Activations;
using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.Services
{
    
    public class ActivationService : IActivationService
    {
        private readonly MyDbContext _db;

        public ActivationService(MyDbContext db)
        {
            _db = db;
        }

        public async Task<CodeActivation?> CreateCodeActivationAsync(CreateActivationDTO dto)
        {
            // 1️⃣ Génération automatique si vide
            var activationCodeRef = $"REF-{Guid.NewGuid():N}".ToUpper();

            var activationCode = Guid.NewGuid().ToString("N").ToUpper();


            // 2️⃣ Conversion DTO -> entité
            var entity = dto.ToEntity();

            // 3️⃣ On remplit les champs générés par le service
            entity.ActivationCodeReference = activationCodeRef;
            entity.ActivationCode = activationCode;

            // 4️⃣ Typage TPH : pas besoin de toucher le discriminant "Type"
            // EF Core détecte automatiquement selon le type instancié.
            // Si tu veux réellement créer un Abonnement ou une Licence :
            if (dto.Type == "Abonnement")
                entity = new Abonnement(entity);   // constructeur spécialisé
            else if (dto.Type == "Licence")
                entity = new Licence(entity);

            // 5️⃣ Enregistrement
            try
            {
                _db.CodeActivations.Add(entity);
                var result = await _db.SaveChangesAsync();
                if (result > 0)
                    return entity;
                else
                    return null; // échec d'ajout
            }
            catch
            {
                return null; // échec avec exception
            }
        }



        // Assign code to email/phone (editor)
        public async Task<(bool Success, string Message)> AssignActivationToUserAsync(AssignActivationDTO dto)
        {
            var code = await _db.CodeActivations
                .FirstOrDefaultAsync(c => c.ActivationCodeReference == dto.ActivationCodeReference || c.ActivationCode == dto.ActivationCodeReference);

            if (code == null)
                return (false, "Code d'activation introuvable.");

            if (!string.IsNullOrWhiteSpace(code.ClientRef) || !string.IsNullOrWhiteSpace(code.AssignedToEmail) || !string.IsNullOrWhiteSpace(code.AssignedToPhone))
                return (false, "Ce code est déjà assigné.");

            if (!string.IsNullOrWhiteSpace(dto.Email))
                code.AssignedToEmail = dto.Email.Trim().ToLowerInvariant();

            if (!string.IsNullOrWhiteSpace(dto.Phone))
                code.AssignedToPhone = dto.Phone.Trim();

            //code.ClientRef ??= dto.ClientRef;

            //code.ActivationDate ??= dto.ActivationDate ?? DateTime.UtcNow;
            //code.ExpirationDate ??= dto.ExpirationDate; // optional

            await _db.SaveChangesAsync();
            return (true, "Code affecté avec succès.");
        }

        // Register device (called by client app when activating)
        public async Task<(bool Success, string Message)> RegisterDeviceAsync(string activationCode, ActivationDeviceDTO device)
        {
            if (string.IsNullOrWhiteSpace(activationCode))
                return (false, "Activation code required.");

            var code = await _db.CodeActivations
                .Include(c => c.ActivationDevices)
                .FirstOrDefaultAsync(c => c.ActivationCode == activationCode || c.ActivationCodeReference == activationCode);

            if (code == null)
                return (false, "Code invalide.");

            if (code.ExpirationDate.HasValue && code.ExpirationDate.Value < DateTime.UtcNow)
                return (false, "Code expiré.");

            // check if device already exists
            var existing = code.ActivationDevices.FirstOrDefault(d => d.DeviceId == device.DeviceId);
            if (existing != null)
            {
                if (!existing.IsActive)
                    return (false, "Appareil désactivé par l'administrateur.");

                existing.LastUsage = DateTime.UtcNow;
                existing.AppVersion = device.AppVersion ?? existing.AppVersion;
                existing.OS = device.OS ?? existing.OS;
                await _db.SaveChangesAsync();
                return (true, "Appareil ré-activé.");
            }

            // respect device quota
            var activeDevicesCount = code.ActivationDevices.Count(d => d.IsActive);
            if (code.MaxDevicesAllowed > 0 && activeDevicesCount >= code.MaxDevicesAllowed)
                return (false, "Nombre maximal d'appareils atteint.");

            var newDevice = new ActivationDevice
            {
                CodeActivationId = code.Id,
                DeviceId = device.DeviceId,
                DeviceName = device.DeviceName,
                OS = device.OS,
                AppVersion = device.AppVersion,
                IsActive = true,
                ActivationDate = DateTime.UtcNow,
                LastUsage = DateTime.UtcNow,
                //ActivatedBy = code.AssignedToEmail ?? code.AssignedToPhone ?? code.ClientRef
            };

            _db.ActivationDevices.Add(newDevice);
            await _db.SaveChangesAsync();
            return (true, "Appareil enregistré.");
        }

        // Validate activation (optionally check device)
        public async Task<(bool Success, string Message, string? AccessType, DateTime? Expiration)> ValidateActivationAsync(string activationCode, string? deviceId = null, string? email = null, string? phone = null)
        {
            if (string.IsNullOrWhiteSpace(activationCode))
                return (false, "Activation code required.", null, null);

            var code = await _db.CodeActivations
                .Include(c => c.ActivationDevices)
                .FirstOrDefaultAsync(c => c.ActivationCode == activationCode || c.ActivationCodeReference == activationCode);

            if (code == null)
                return (false, "Code invalide.", null, null);

            // assigned checks
            if (!string.IsNullOrWhiteSpace(code.AssignedToEmail) && !string.IsNullOrWhiteSpace(email))
            {
                if (!string.Equals(code.AssignedToEmail, email.Trim(), StringComparison.OrdinalIgnoreCase))
                    return (false, "Code non affecté à cet e‑mail.", null, code.ExpirationDate);
            }

            if (!string.IsNullOrWhiteSpace(code.AssignedToPhone) && !string.IsNullOrWhiteSpace(phone))
            {
                if (!string.Equals(code.AssignedToPhone, phone.Trim(), StringComparison.OrdinalIgnoreCase))
                    return (false, "Code non affecté à ce numéro.", null, code.ExpirationDate);
            }

            if (code.ExpirationDate.HasValue && code.ExpirationDate.Value < DateTime.UtcNow)
                return (false, "Code expiré.", null, code.ExpirationDate);

            // device checks
            if (!string.IsNullOrWhiteSpace(deviceId))
            {
                var device = code.ActivationDevices.FirstOrDefault(d => d.DeviceId == deviceId);
                if (device == null)
                    return (false, "Appareil non enregistré pour ce code.", null, code.ExpirationDate);

                if (!device.IsActive)
                    return (false, "Appareil désactivé.", null, code.ExpirationDate);

                // update last usage
                device.LastUsage = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }

            // determine type (Abonnement | Licence)
            string? accessType = null;
            DateTime? expiration = code.ExpirationDate;

            var abo = await _db.Abonnements.FindAsync(code.Id);
            if (abo != null)
            {
                accessType = "Abonnement";
                expiration = abo.ExpirationDate ?? expiration;
                if (abo.IsActive == false)
                    return (false, "Abonnement inactif.", accessType, expiration);
            }
            else
            {
                var lic = await _db.Licences.FindAsync(code.Id);
                if (lic != null)
                {
                    accessType = "Licence";
                    expiration = lic.ExpirationDate ?? expiration;
                    if (lic.IsActive == false)
                        return (false, "Licence inactive.", accessType, expiration);
                }
            }

            return (true, "Code valide.", accessType, expiration);
        }

        // Helper for middleware: get access via email/phone
        public async Task<ValidateActivationResultDTO> GetAccessForUserAsync(string? email, string? phone)
        {
            if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(phone))
                return new ValidateActivationResultDTO { IsValid = false, Message = "Aucun identifiant fourni." };

            email = email?.Trim().ToLowerInvariant();
            phone = phone?.Trim();

            var code = await _db.CodeActivations
                .Include(c => c.ActivationDevices)
                .FirstOrDefaultAsync(c => (!string.IsNullOrEmpty(email) && c.AssignedToEmail == email) || (!string.IsNullOrEmpty(phone) && c.AssignedToPhone == phone) || (c.ClientRef != null && (c.ClientRef == email || c.ClientRef == phone)));

            if (code == null)
                return new ValidateActivationResultDTO { IsValid = false, Message = "Aucun code assigné." };

            var (success, message, accessType, expiration) = await ValidateActivationAsync(code.ActivationCode, null, email, phone);
            return new ValidateActivationResultDTO
            {
                IsValid = success,
                Message = message,
                AccessType = accessType,
                ExpirationDate = expiration
            };
        }

        public async Task<CodeActivation?> GetByReferenceAsync(string reference)
        {
            return await _db.CodeActivations.FirstOrDefaultAsync(c => c.ActivationCode == reference || c.ActivationCodeReference == reference);
        }
    }
}
