using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.DTO.SIsDTOs
{
    public class SIUpdateDTO
    {
        public string? Name { get; set; }        
        public string? Description { get; set; }
        public string? Adresse_Image_SI { get; set; }



        public SIUpdateDTO() { }


        public static SIUpdateDTO FromSIEntity(SI si)
        {
            return new SIUpdateDTO
            {
                Name = si.Name,
                Description = si.Description,
                Adresse_Image_SI = si.Adresse_Image_SI,

            };
        }


        public SI ToSIEntity()
        {
            return new SI
            {
                Name = Name,
                
                Description = Description,
                Adresse_Image_SI = Adresse_Image_SI,
                
            };
        }
    }
}
