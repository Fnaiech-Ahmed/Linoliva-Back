using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using tech_software_engineer_consultant_int_backend.DTO;
using tech_software_engineer_consultant_int_backend.DTO.Activations;
using tech_software_engineer_consultant_int_backend.DTO.ActivationDTOs;
using tech_software_engineer_consultant_int_backend.Models;
using tech_software_engineer_consultant_int_backend.DTO.ActivationCodesDTOs;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public interface IActivationService
    {
        Task<CodeActivation?> CreateCodeActivationAsync(CreateActivationDTO code);
        Task<(bool Success, string Message)> AssignActivationToUserAsync(AssignActivationDTO dto);
        Task<(bool Success, string Message)> RegisterDeviceAsync(string activationCode, ActivationDeviceDTO device);
        Task<(bool Success, string Message, string? AccessType, DateTime? Expiration)> ValidateActivationAsync(string activationCode, string? deviceId = null, string? email = null, string? phone = null);
        Task<ValidateActivationResultDTO> GetAccessForUserAsync(string? email, string? phone);
        Task<CodeActivation?> GetByReferenceAsync(string reference);
    }
}