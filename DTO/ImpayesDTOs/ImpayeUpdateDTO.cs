using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.DTO.ImpayesDTOs
{
    public class ImpayeUpdateDTO
    {
        public string? RefClt { get; set; }
        public string? Nom { get; set; }
        public string? Prenom { get; set; }
        public string? Createdate { get; set; }
        public string? Datetombe { get; set; }
        public string? Retard { get; set; }
        public string? Statut { get; set; }
        public string? Priorite { get; set; }
        public string? Montant { get; set; }


        public ImpayeUpdateDTO() { }


        public ImpayeUpdateDTO(Impaye impaye)
        {
            RefClt = impaye.RefClt;
            Nom = impaye.Nom;
            Prenom = impaye.Prenom;
            Createdate = impaye.Createdate;
            Datetombe = impaye.Datetombe;
            Retard = impaye.Retard; Statut = impaye.Statut;
            Priorite = impaye.Priorite;
            Montant = impaye.Montant;
        }


        public Impaye ToImpayeEntity()
        {
            return new Impaye
            {
                RefClt = RefClt,
                Nom = Nom,
                Prenom = Prenom,
                Createdate = Createdate,
                Datetombe = Datetombe,
                Retard = Retard,
                Statut = Statut,
                Priorite = Priorite,
                Montant = Montant,
            };
        }
    }
}
