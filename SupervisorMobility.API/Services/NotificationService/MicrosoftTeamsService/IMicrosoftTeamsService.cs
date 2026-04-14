namespace SupervisorMobility.API.Services.MicrosoftTeamsService
{
    public interface IMicrosoftTeamsService
    {
        Task<bool> SendMicrosoftTeamsMessageAsync(string recipientId, string message);
    }
}
