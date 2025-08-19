using System.Text;

namespace GestionConge.Client.Helpers
{
    public static class ExportHelper
    {
        /// <summary>
        /// Exporte les données en CSV
        /// </summary>
        public static string ToCsv<T>(IEnumerable<T> data, Dictionary<string, Func<T, object>> columns)
        {
            var csv = new StringBuilder();

            // En-têtes
            csv.AppendLine(string.Join(";", columns.Keys));

            // Données
            foreach (var item in data)
            {
                var values = columns.Values.Select(func => func(item)?.ToString() ?? "");
                csv.AppendLine(string.Join(";", values));
            }

            return csv.ToString();
        }

        /// <summary>
        /// Génère un nom de fichier avec timestamp
        /// </summary>
        public static string GenerateFileName(string baseName, string extension = "csv")
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            return $"{baseName}_{timestamp}.{extension}";
        }
    }

}
