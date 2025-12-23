
using System;
using System.Globalization;

namespace tech_software_engineer_consultant_int_backend.Services
{
    public class CalculRangSemaineServices
    {
        // Méthode pour retourner le tuple avec le rang de la semaine dans l'année correcte
        public static (int semaineAnnee, int annee, decimal valeur) GenererTupleSemaine(DateTime startDate, DateTime endDate, int chosenWeek, decimal valeurSemaine)
        {
            // Calculer les semaines dans chaque année de l'intervalle
            int weeksInFirstYear = CalculateWeeksInYear(startDate, new DateTime(startDate.Year, 12, 31));
            int weeksInSecondYear = CalculateWeeksInYear(new DateTime(endDate.Year, 1, 1), endDate);

            // Cas où la semaine choisie se trouve dans la première année
            if (chosenWeek <= weeksInFirstYear)
            {
                // Calculer la date de la semaine choisie dans la première année
                DateTime selectedDate = startDate.AddDays((chosenWeek - 1) * 7);
                int weekOfYear = GetWeekOfYear(selectedDate);

                return (weekOfYear, startDate.Year, valeurSemaine);
            }
            // Cas où la semaine choisie se trouve dans la deuxième année
            else if (chosenWeek <= weeksInFirstYear + weeksInSecondYear)
            {
                // Ajuster chosenWeek pour la deuxième année
                int adjustedWeek = chosenWeek - weeksInFirstYear;
                DateTime selectedDate = new DateTime(endDate.Year, 1, 1).AddDays((adjustedWeek - 1) * 7);
                int weekOfYear = GetWeekOfYear(selectedDate);

                return (weekOfYear, endDate.Year, valeurSemaine);
            }
            else
            {
                throw new ArgumentException("Le numéro de semaine sélectionné est invalide.");
            }
        }

        // Calcul du nombre de semaines dans une année donnée
        public static int CalculateWeeksInYear(DateTime start, DateTime end)
        {
            TimeSpan span = end - start;
            return (int)Math.Ceiling(span.TotalDays / 7.0);
        }

        // Fonction pour obtenir la position de la semaine dans l'année
        public static int GetWeekOfYear(DateTime date)
        {
            CultureInfo ci = CultureInfo.InvariantCulture; // Utilisation de la culture invariante pour cohérence
            Calendar calendar = ci.Calendar;

            // Règle : La première semaine doit contenir au moins 4 jours (ISO 8601)
            CalendarWeekRule weekRule = CalendarWeekRule.FirstFourDayWeek;
            DayOfWeek firstDayOfWeek = DayOfWeek.Monday; // Fixer le premier jour de la semaine au lundi

            return calendar.GetWeekOfYear(date, weekRule, firstDayOfWeek);
        }



        /// <summary>
        /// Retourne une liste des semaines formatées comme "Semaine-Année" pour chaque semaine entre deux dates spécifiées.
        /// La liste est triée en ordre croissant.
        /// </summary>
        /// <param name="startDate">La date de début à partir de laquelle les semaines seront calculées.</param>
        /// <param name="endDate">La date de fin jusqu'à laquelle les semaines seront calculées.</param>
        /// <returns>Une liste de chaînes représentant les semaines sous la forme "Week-Année".</returns>
        public static List<string> GetRealWeekRanks(DateTime startDate, DateTime endDate)
        {
            var weekRanks = new List<string>();
            int totalWeeks = (int)Math.Ceiling((endDate - startDate).TotalDays / 7); // Calculer le nombre total de semaines

            for (int week = 0; week < totalWeeks; week++)
            {
                // Avancer de semaine en semaine en ajoutant 7 jours
                DateTime currentWeek = startDate.AddDays(week * 7);

                // Si la semaine dépasse la date de fin, on arrête
                if (currentWeek > endDate) break;

                // Obtenir le numéro de la semaine
                int weekOfYear = GetWeekOfYear(currentWeek);

                // Formatage de la semaine avec un zéro pour les semaines < 10 (ex: "02-2024")
                string formattedWeek = $"{weekOfYear:D2}-{currentWeek.Year}";

                // Ajouter le format "Week-Année" à la liste
                weekRanks.Add(formattedWeek);
            }

            // Trier la liste en ordre croissant
            weekRanks.Sort();

            return weekRanks;
        }
    }
}
