using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.DTO.PortfoliosDTOs
{
    public class PortefeuilleCreateDTO
    {
        public string Nom { get; set; }
        public string Date { get; set; }
        public string Action { get; set; }



        // Ensure a default parameterless constructor
        public PortefeuilleCreateDTO()
        {
        }

        // Méthode pour convertir PortefeuilleDto en entité Portefeuille
        public Portefeuille ToPortefeuilleEntity()
        {
            return new Portefeuille
            {
                Nom = Nom,
                Date = Date,
                Action = Action
            };
        }

        // Méthode pour convertir entité Portefeuille en PortefeuilleDto
        public PortefeuilleCreateDTO(Portefeuille portefeuille)
        {
            Nom = portefeuille.Nom;
            Action = portefeuille.Action;
            Date = portefeuille.Date;
        }

    }
}
