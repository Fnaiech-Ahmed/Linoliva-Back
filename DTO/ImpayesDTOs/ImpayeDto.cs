using tech_software_engineer_consultant_int_backend.DTO.DossiersDTOs;
using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.DTO.ImpayesDTOs
{
    public class ImpayeDto
    {
        public int id { get; set; }
        public string Ref { get; set; } = "";
        public string RefClt { get; set; } = "";
        public string Nom { get; set; } = "";
        public string Prenom { get; set; } = "";
        public string Createdate { get; set; } = "";
        public string Datetombe { get; set; } = "";
        public string Retard { get; set; } = "";
        public string Statut { get; set; } = "";
        public string Priorite { get; set; } = "";
        public int NbreImpaye { get; set; }
        public string Montant { get; set; } = "";

        public int? DossierId { get; set; }
        public DossierDto? Dossier { get; set; }

        public int? EcheanceId { get; set; }
        public EcheanceDto? Echeance { get; set; }


        public ImpayeDto() { }


        public static ImpayeDto fromImpayeEntity(Impaye impaye)
        {
            return new ImpayeDto
            {
                id = impaye.id,
                Ref = impaye.Ref,
                RefClt = impaye.RefClt,
                Nom = impaye.Nom,
                Prenom = impaye.Prenom,
                Createdate = impaye.Createdate,
                Datetombe = impaye.Datetombe,
                Retard = impaye.Retard,
                Statut = impaye.Statut,
                Priorite = impaye.Priorite,
                NbreImpaye = impaye.NbreImpaye,
                Montant = impaye.Montant,
                
                DossierId = impaye.DossierId,  
                
                EcheanceId = impaye.EcheanceId,
            };
        }


        public Impaye ToImpayeEntity()
        {
            return new Impaye
            {
                id = id,
                Ref = Ref,
                RefClt = RefClt,
                Nom = Nom,
                Prenom = Prenom,
                Createdate = Createdate,
                Datetombe = Datetombe,
                Retard = Retard,
                Statut = Statut,
                Priorite = Priorite,
                NbreImpaye = NbreImpaye,
                Montant = Montant,

                DossierId = DossierId,

                EcheanceId = EcheanceId,
            };
        }
    }
}
