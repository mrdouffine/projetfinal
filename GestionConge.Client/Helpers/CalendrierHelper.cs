namespace GestionConge.Client.Helpers
{
    public static class CalendrierHelper
    {
        /// <summary>
        /// Génère les jours d'un mois pour l'affichage calendrier
        /// </summary>
        public static List<DateTime> GetJoursMois(int annee, int mois)
        {
            var jours = new List<DateTime>();
            var premierJour = new DateTime(annee, mois, 1);
            var dernierJour = premierJour.AddMonths(1).AddDays(-1);

            // Ajouter les jours du mois précédent pour compléter la semaine
            var premierLundi = premierJour.AddDays(-(int)premierJour.DayOfWeek + 1);
            if (premierLundi > premierJour)
                premierLundi = premierLundi.AddDays(-7);

            var current = premierLundi;
            while (current <= dernierJour.AddDays(6 - (int)dernierJour.DayOfWeek))
            {
                jours.Add(current);
                current = current.AddDays(1);
            }

            return jours;
        }

        /// <summary>
        /// Obtient les noms des mois en français
        /// </summary>
        public static Dictionary<int, string> GetNomsMois()
        {
            return new Dictionary<int, string>
            {
                { 1, "Janvier" }, { 2, "Février" }, { 3, "Mars" }, { 4, "Avril" },
                { 5, "Mai" }, { 6, "Juin" }, { 7, "Juillet" }, { 8, "Août" },
                { 9, "Septembre" }, { 10, "Octobre" }, { 11, "Novembre" }, { 12, "Décembre" }
            };
        }

        /// <summary>
        /// Obtient les noms des jours en français
        /// </summary>
        public static Dictionary<DayOfWeek, string> GetNomsJours()
        {
            return new Dictionary<DayOfWeek, string>
            {
                { DayOfWeek.Monday, "Lun" }, { DayOfWeek.Tuesday, "Mar" },
                { DayOfWeek.Wednesday, "Mer" }, { DayOfWeek.Thursday, "Jeu" },
                { DayOfWeek.Friday, "Ven" }, { DayOfWeek.Saturday, "Sam" },
                { DayOfWeek.Sunday, "Dim" }
            };
        }
    }

}
