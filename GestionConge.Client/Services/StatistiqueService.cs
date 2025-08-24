using GestionConge.Client.Models;
using GestionConge.Client.Components;

namespace GestionConge.Client.Services
{
    public class StatistiqueService
    {
        public StatistiqueService() { }

        // Méthode pour obtenir les statistiques (exemple fictif)
        //GetStatistiquesAsync
        public Task<StatistiquesDto> GetStatistiquesAsync()
        {
            // Exemple fictif de données statistiques
            var statistiques = new StatistiquesDto
            {
                TotalEmployes = 150,
                TotalDemandesConge = 75,
                TotalDemandesApprouvees = 50,
                TotalDemandesRefusees = 15,
                TotalDemandesEnAttente = 10,
                TotalCongesPris = 200,
                TotalCongesRestants = 100,
                TotalCongesConge = 300,
                ChartData = new ChartComponent.ChartDataItem
                {
                    //Ne contient pas de définition de DataSets ou de Labels pour ChartDataItem
                    //utilise autre chose pour ChartDataItem à la place de DataSets et Labels



                },
                ChartLabels = new List<string> { "Jan", "Feb", "Mar", "Apr", "May", "Jun" }
            };
            return Task.FromResult(statistiques);
        }
    }
}
