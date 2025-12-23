using System;
using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.DTO.ActivationDTOs
{
    public class CreateActivationDTO
    {
        public int ProductId { get; set; }
        public string BusinessSector { get; set; }
        public int MaxUsersAllowed { get; set; }
        public int MaxDevicesAllowed { get; set; }
        public string Type { get; set; } // "Abonnement" ou "Licence"

        // ------- Conversion DTO -> ENTITÉ -------
        public CodeActivation ToEntity()
        {
            return new CodeActivation
            {
                ProductId = this.ProductId,
                BusinessSector = this.BusinessSector,
                MaxDevicesAllowed = this.MaxDevicesAllowed,
                MaxUsersAllowed = this.MaxUsersAllowed,
                // le service remplira :
                // ActivationCode, ActivationCodeReference, ActivationDate, ExpirationDate
                // TPH géré automatiquement via EF Core
            };
        }

        // ------- Conversion ENTITÉ -> DTO -------
        public static CreateActivationDTO FromEntity(CodeActivation entity)
        {
            return new CreateActivationDTO
            {
                ProductId = entity.ProductId,
                BusinessSector = entity.BusinessSector,
                MaxDevicesAllowed = entity.MaxDevicesAllowed,
                MaxUsersAllowed = entity.MaxUsersAllowed,
                Type = entity is Abonnement ? "Abonnement" : "Licence"
            };
        }
    }
}
