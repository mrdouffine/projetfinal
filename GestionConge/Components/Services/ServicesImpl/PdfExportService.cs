namespace GestionConge.Components.Services.ServicesImpl;

using DinkToPdf;
using DinkToPdf.Contracts;
using GestionConge.Components.Services.IServices;

public class PdfExportService : IPdfExportService
{
    private readonly IConverter _converter;

    public PdfExportService(IConverter converter)
    {
        _converter = converter;
    }

    public byte[] ExportStatistiquesEnPdf()
    {
        var html = @"<h1>Statistiques des Congés</h1>
                     <ul>
                       <li>Congés par mois: ...</li>
                       <li>Solde utilisateur: ...</li>
                       <li>Pic d'absences: ...</li>
                     </ul>";

        var doc = new HtmlToPdfDocument()
        {
            GlobalSettings = {
            PaperSize = PaperKind.A4,
            Orientation = Orientation.Portrait
        },
            Objects = {
            new ObjectSettings() {
                HtmlContent = html
            }
        }
        };

        return _converter.Convert(doc);
    }
}
