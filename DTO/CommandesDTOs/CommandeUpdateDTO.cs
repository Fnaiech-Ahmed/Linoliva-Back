using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.DTO.CommandesDTOs
{
    public class CommandeUpdateDTO
    {
        public string ReferenceCommande { get; set; }
        public string NomVendeur { get; set; }
        public string AdresseVendeur { get; set; }
        public string MFVendeur { get; set; }
        public long GSMVendeur { get; set; }

        public Titre TitreAcheteur { get; set; }
        public string NomAcheteur { get; set; }
        public string AdresseAcheteur { get; set; }
        public string MFAcheteur { get; set; }
        public long GSMAcheteur { get; set; }
        public int? ProprietaireId { get; set; }


        public decimal TVA { get; set; }



        public CommandeUpdateDTO() { }


        public static CommandeUpdateDTO FromCommandeEntity(Commande commande)
        {
            return new CommandeUpdateDTO
            {
                ReferenceCommande = commande.ReferenceCommande,
                NomVendeur = commande.NomVendeur,
                AdresseVendeur = commande.AdresseVendeur,
                MFVendeur = commande.MFVendeur,
                GSMVendeur = commande.GSMVendeur,

                TitreAcheteur = commande.TitreAcheteur,
                NomAcheteur = commande.NomAcheteur,
                AdresseAcheteur = commande.AdresseAcheteur,
                MFAcheteur = commande.MFAcheteur,
                GSMAcheteur = commande.GSMAcheteur,
                ProprietaireId = commande.ProprietaireId,

                TVA = commande.TVA,
            };
        }


        public Commande ToCommandeEntity()
        {
            return new Commande
            {
                ReferenceCommande= ReferenceCommande,
                NomVendeur = NomVendeur,
                AdresseVendeur = AdresseVendeur,
                MFVendeur = MFVendeur,
                GSMVendeur = GSMVendeur,

                TitreAcheteur = TitreAcheteur,
                NomAcheteur = NomAcheteur,
                AdresseAcheteur = AdresseAcheteur,
                MFAcheteur = MFAcheteur,
                GSMAcheteur = GSMAcheteur,
                ProprietaireId = ProprietaireId,

                TVA = TVA,

            };
        }
    }
}
