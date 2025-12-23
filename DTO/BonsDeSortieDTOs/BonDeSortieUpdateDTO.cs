using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.DTO.BonsDeSortieDTOs
{
    public class BonDeSortieUpdateDTO
    {
        public List<Facture> ListeFactures { get; set; }

        public string MatriculeDeVoiture { get; set; }
        public DateTime DateDebutCirculation { get; set; }
        public DateTime DateFinCirculation { get; set; }


        public BonDeSortie ToBSEntity()
        {
            return new BonDeSortie()
            {
                ListeFactures = ListeFactures,
                MatriculeDeVoiture = MatriculeDeVoiture,
                DateDebutCirculation = DateDebutCirculation,
                DateFinCirculation = DateFinCirculation,
            };
        }


        public static BonDeSortieUpdateDTO FromBSEntity (BonDeSortie entity)
        {
            return new BonDeSortieUpdateDTO()
            {
                ListeFactures = entity.ListeFactures,
                MatriculeDeVoiture = entity.MatriculeDeVoiture,
                DateDebutCirculation = entity.DateDebutCirculation,
                DateFinCirculation = entity.DateFinCirculation
            };
        }
    }
}
