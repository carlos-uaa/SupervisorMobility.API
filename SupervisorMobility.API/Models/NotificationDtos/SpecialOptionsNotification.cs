namespace SupervisorMobility.API.Models.NotificationDtos
{
    public class SpecialOptionsNotification
    {
        public bool? Email { get; set; }
        public bool? WhatsApp { get; set; }
        public bool? MicrosoftTeams { get; set; }
        public string? type { get; set; }
    }
}
