namespace GestionConge.Client.Models
{
    public class RefreshRequestDto
    {
        public string AccessToken { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
    }
}
