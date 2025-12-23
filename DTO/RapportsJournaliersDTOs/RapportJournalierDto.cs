using tech_software_engineer_consultant_int_backend.DTO.RecettesDTOs;
using tech_software_engineer_consultant_int_backend.DTO.DepensesDTOs;

namespace tech_software_engineer_consultant_int_backend.DTO.RapportsJournaliersDTOs
{
    public class RapportJournalierDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public List<RecetteDto> Recettes { get; set; }
        public List<DepenseDto> Depenses { get; set; }
    }
}
