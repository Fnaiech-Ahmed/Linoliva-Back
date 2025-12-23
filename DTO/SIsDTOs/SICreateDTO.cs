using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.DTO.SIsDTOs
{
    public class SICreateDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Adresse_Image_SI { get; set; }





        public SICreateDTO() { }


        public SICreateDTO(SI si)
        {
            this.Name = si.Name;
            this.Description = si.Description;
            this.Adresse_Image_SI = si.Adresse_Image_SI;
        }


        public SI ToSIEntity()
        {
            return new SI
            {
                Name = this.Name,
                
                Description = this.Description,
                Adresse_Image_SI = this.Adresse_Image_SI,
                
            };
        }
    }
}
