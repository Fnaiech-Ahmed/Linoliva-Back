using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.DTO.BonsDeSortieDTOs
{
    public class BonDeSortieCreateDTO
    {
        public List<Facture> ListeFactures { get; set; }

        public string MatriculeDeVoiture { get; set; }
        public DateTime DateDebutCirculation { get; set; }
        public DateTime DateFinCirculation { get; set; }


        public BonDeSortieCreateDTO() { }


        public static BonDeSortieCreateDTO FromBSEntity(BonDeSortie entity) {
            return new BonDeSortieCreateDTO()
            {
                ListeFactures = entity.ListeFactures,
                MatriculeDeVoiture = entity.MatriculeDeVoiture,
                DateDebutCirculation = entity.DateDebutCirculation,
                DateFinCirculation = entity.DateFinCirculation
            };
        }


        public BonDeSortie ToBSEntity() { 
            return new BonDeSortie() { 
                ListeFactures = ListeFactures,
                MatriculeDeVoiture = MatriculeDeVoiture,
                DateDebutCirculation = DateDebutCirculation, 
                DateFinCirculation = DateFinCirculation,                
            }; 
        }
    }
}
