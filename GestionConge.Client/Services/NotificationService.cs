public class NotificationService
{
    private readonly HttpClient _http;
    public NotificationService(HttpClient http) { _http = http; }

    public async Task<bool> SendPostApprovalEmailAsync(int demandeId)
    {
        var resp = await _http.PostAsync($"api/notifications/postApproval/{demandeId}", null);
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> ScheduleReturnReminderAsync(int demandeId)
    {
        var resp = await _http.PostAsync($"api/notifications/scheduleReturnReminder/{demandeId}", null);
        return resp.IsSuccessStatusCode;
    }
}
