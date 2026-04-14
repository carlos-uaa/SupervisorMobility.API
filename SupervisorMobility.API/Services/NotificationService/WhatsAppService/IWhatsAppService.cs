namespace SupervisorMobility.API.Services.WhatsAppService
{
    public interface IWhatsAppService
    {
        Task<bool> SendWhatsAppTemplateAsync(string recipientPhoneNumber, string whatsAppTemplate);
    }
}
