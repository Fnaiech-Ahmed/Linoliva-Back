using System.ComponentModel.DataAnnotations.Schema;
using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.DTO.BonsDeSortieDTOs
{
    public class BonDeSortieDTO
    {
        public int Id { get; set; }
        public string ReferenceBS { get; set; }

        [NotMapped]
        public List<Facture> ListeFactures { get; set; }

        public string MatriculeDeVoiture { get; set; }
        public DateTime DateDebutCirculation { get; set; }
        public DateTime DateFinCirculation { get; set; }





        public BonDeSortieDTO() { }



        public BonDeSortie ToBSEntity()
        {
            return new BonDeSortie()
            {
                Id = Id,
                ReferenceBS = ReferenceBS,
                ListeFactures = ListeFactures,
                MatriculeDeVoiture = MatriculeDeVoiture,
                DateDebutCirculation = DateDebutCirculation,
                DateFinCirculation = DateFinCirculation,
            };
        }


        public static BonDeSortieDTO FromBSEntity(BonDeSortie entity)
        {
            return new BonDeSortieDTO()
            {
                Id = entity.Id,
                ReferenceBS = entity.ReferenceBS,
                ListeFactures = entity.ListeFactures,
                MatriculeDeVoiture = entity.MatriculeDeVoiture,
                DateDebutCirculation = entity.DateDebutCirculation,
                DateFinCirculation = entity.DateFinCirculation
            };
        }
    }
}
