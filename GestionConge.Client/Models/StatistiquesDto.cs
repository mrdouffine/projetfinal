using GestionConge.Client.Components;

namespace GestionConge.Client.Models;

public class StatistiquesDto
{
    public int Id { get; set; }

    public int TotalEmployes { get; set; }
    public int TotalDemandesConge { get; set; }
    public int TotalDemandesApprouvees { get; set; }
    public int TotalDemandesRefusees { get; set; }
    public int TotalDemandesEnAttente { get; set; }
    public int TotalCongesPris { get; set; }
    public int TotalCongesRestants { get; set; }
    public int TotalCongesConge
    {
        get;

        set;
    }

    //ChartData variable champ
    public ChartComponent.ChartDataItem ChartData { get; set; }
    public List<string> ChartLabels { get; set; } = new List<string>();


}
