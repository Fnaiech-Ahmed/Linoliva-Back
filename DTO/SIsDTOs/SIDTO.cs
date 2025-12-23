using tech_software_engineer_consultant_int_backend.DTO.ProductDTOs;
using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.DTO.SIsDTOs
{
    public class SIDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Adresse_Image_SI { get; set; }





        public SIDTO() { }


        public static SIDTO FromSIEntity(SI si)
        {
            return new SIDTO
            {
                Id = si.Id,
                Name = si.Name,
                
                Description = si.Description,
                Adresse_Image_SI = si.Adresse_Image_SI,

            };
        }



        public SI ToSIEntity()
        {
            return new SI
            {
                Id = Id,
                Name = Name,
                Description = Description,
                Adresse_Image_SI = Adresse_Image_SI,
                
            };
        }
    }
}
