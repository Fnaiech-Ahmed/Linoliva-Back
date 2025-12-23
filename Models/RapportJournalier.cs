namespace tech_software_engineer_consultant_int_backend.Models
{
    public class RapportJournalier
    {
        public int Id { get; set; }
        private DateTime _date;
        public DateTime Date
        {
            get => _date.Date; // Retourne uniquement la partie date (sans l'heure)
            set => _date = value.Date; // Enregistre uniquement la date avec l'heure à 00:00:00
        }

        public List<Recette> Recettes { get; set; } // 3 recettes provenant des 3 postes
        public List<Depense> Depenses { get; set; } // Les dépenses du jour

        /*
         * public int UtilisateurId { get; set; }
         * public Utilisateur Utilisateur { get; set; }
        */
    }

}
