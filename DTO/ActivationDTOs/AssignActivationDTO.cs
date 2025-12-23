using tech_software_engineer_consultant_int_backend.Models;


namespace tech_software_engineer_consultant_int_backend.DTO.ActivationDTOs
{
    public class AssignActivationDTO
    {
        public string ActivationCodeReference { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string? AssignedBy { get; set; }              // L'admin / vendeur qui assigne le code

        // ------- Conversion DTO -> ENTITÉ (partielle) -------
        public void MapToEntity(CodeActivation entity)
        {
            entity.AssignedToEmail = this.Email;
            entity.AssignedToPhone = this.Phone;
            //entity.IsAssigned = true;
        }

        // ------- Conversion ENTITÉ -> DTO -------
        public static AssignActivationDTO FromEntity(CodeActivation entity)
        {
            return new AssignActivationDTO
            {
                ActivationCodeReference = entity.ActivationCodeReference,
                Email = entity.AssignedToEmail,
                Phone = entity.AssignedToPhone
            };
        }
    }
}
