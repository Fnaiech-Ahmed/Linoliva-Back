namespace tech_software_engineer_consultant_int_backend.DTO.RecettesDTOs
{
    public class RecetteEqpDayKey
    {
        public int PosteRecetteId { get; set; }
        public List<Dictionary<int, decimal>> DaysMontants { get; set; }
    }
}
