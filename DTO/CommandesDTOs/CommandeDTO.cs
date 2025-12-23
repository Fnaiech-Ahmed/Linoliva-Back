using tech_software_engineer_consultant_int_backend.Models;

namespace tech_software_engineer_consultant_int_backend.DTO.CommandesDTOs
{
    public class CommandeDTO
    {
        public int Id { get; set; }
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

        public List<int> ListIdsTransactions { get; set; }

        public decimal MontantTotalHT { get; set; }
        public decimal TVA { get; set; }
        public decimal MontantTotalTTC { get; set; }



        public CommandeDTO()
        {

        }


        public static CommandeDTO FromCommandeEntity(Commande commande)
        {
            return new CommandeDTO
            {
                Id = commande.Id,
                ReferenceCommande = commande.ReferenceCommande,

                NomVendeur = commande.NomVendeur,
                AdresseVendeur = commande.AdresseVendeur, 
                MFVendeur = commande.MFVendeur,
                GSMVendeur= commande.GSMVendeur,

                TitreAcheteur = commande.TitreAcheteur,
                NomAcheteur= commande.NomAcheteur,
                AdresseAcheteur = commande.AdresseAcheteur,
                MFAcheteur = commande.MFAcheteur,
                GSMAcheteur = commande.GSMAcheteur,
                ProprietaireId = commande.ProprietaireId,

                ListIdsTransactions = commande.ListIdsTransactions,

                MontantTotalHT = commande.MontantTotalHT,
                TVA = commande.TVA,
                MontantTotalTTC = commande.MontantTotalTTC,

            };
        }


        public Commande ToCommandeEntity()
        {
            return new Commande
            {
                Id = Id,
                ReferenceCommande = ReferenceCommande,

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

                ListIdsTransactions = ListIdsTransactions,

                MontantTotalHT = MontantTotalHT,
                TVA = TVA,
                MontantTotalTTC = MontantTotalTTC,
            };
        }
    }
}
