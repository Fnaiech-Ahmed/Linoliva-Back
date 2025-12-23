namespace tech_software_engineer_consultant_int_backend.DTO.RecettesDTOs
{
    public class RecetteEqpWeekKey
    {
        public int PosteRecetteId { get; set; }
        public List<Dictionary<string, decimal>> WeeksMontants { get; set; }
    }
}
