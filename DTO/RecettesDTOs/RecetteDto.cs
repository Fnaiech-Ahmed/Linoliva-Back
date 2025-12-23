using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.DTO.RecettesDTOs
{
    public class RecetteDto
    {
        public int Id { get; set; }
        public decimal Montant { get; set; }
        public int PosteRecetteId { get; set; }
        public DateTime DateTransaction { get; set; }
        public int? RapportJournalierId { get; set; }





        // Méthode de transformation vers RecetteDto
        public static RecetteDto ToDto(Recette recette)
        {
            return new RecetteDto
            {
                Id = recette.Id,
                Montant = recette.Montant,
                PosteRecetteId = recette.PosteRecetteId,
                DateTransaction = recette.DateTransaction,
                RapportJournalierId = recette.RapportJournalierId
            };
        }

        // Méthode de transformation depuis RecetteDto
        public Recette ToRecetteEntity()
        {
            return new Recette
            {
                Id = Id,
                Montant = Montant,
                PosteRecetteId = PosteRecetteId,
                DateTransaction = DateTransaction,
                RapportJournalierId = RapportJournalierId
            };
        }
    }
}
