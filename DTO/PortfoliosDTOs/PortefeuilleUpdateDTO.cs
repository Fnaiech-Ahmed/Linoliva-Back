using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.DTO.PortfoliosDTOs
{
    public class PortefeuilleUpdateDTO
    {
        public string Nom { get; set; }
        public string Date { get; set; }
        public string Action { get; set; }


        // Méthode pour convertir PortefeuilleDto en entité Portefeuille
        public Portefeuille ToPortefeuilleEntity(PortefeuilleDto portefeuilleDto)
        {
            return new Portefeuille
            {
                Nom = portefeuilleDto.Nom,
                Date = portefeuilleDto.Date,
                Action = portefeuilleDto.Action
            };
        }

        // Méthode pour convertir entité Portefeuille en PortefeuilleDto
        public PortefeuilleUpdateDTO(Portefeuille portefeuille)
        {
            Nom = portefeuille.Nom;
            Action = portefeuille.Action;
            Date = portefeuille.Date;
        }
    }
}
