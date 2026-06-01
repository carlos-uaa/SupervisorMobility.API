using System.Net.Http.Headers;
using System.Net.Http.Json;
using SupervisorMobility.API.DataAccess.Entities;

namespace SupervisorMobility.API.Services.WhatsAppService
{
    public class WhatsAppService : IWhatsAppService
    {
        private readonly WhatsAppConfiguration _whatsAppConfig;
        private readonly HttpClient _httpClient;

        public WhatsAppService(
            WhatsAppConfiguration whatsAppConfig,
            IHttpClientFactory httpClientFactory)
        {
            _whatsAppConfig = whatsAppConfig;
            _httpClient = httpClientFactory.CreateClient();
        }


        public async Task<bool> SendWhatsAppTemplateAsync(string recipientPhoneNumber, string whatsAppTemplate)
        {
            if (string.IsNullOrWhiteSpace(recipientPhoneNumber) || string.IsNullOrWhiteSpace(whatsAppTemplate))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(_whatsAppConfig.UserAccessToken) ||
                string.IsNullOrWhiteSpace(_whatsAppConfig.Version) ||
                string.IsNullOrWhiteSpace(_whatsAppConfig.PhoneNumberId))
            {
                return false;
            }

            var facebookGraphApiUrl =
                $"https://graph.facebook.com/{_whatsAppConfig.Version}/{_whatsAppConfig.PhoneNumberId}/messages";

            var payload = new
            {
                messaging_product = "whatsapp",
                recipient_type = "individual",
                to = recipientPhoneNumber,
                type = "template",
                template = new
                {
                    name = whatsAppTemplate,
                    language = new
                    {
                        code = "en_US"
                    },
                    components = new object[]
                    {
                        new {
                            type = "body",
                            parameters = Array.Empty<object>()
                        }
                    }
                }
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, facebookGraphApiUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _whatsAppConfig.UserAccessToken);
            request.Content = JsonContent.Create(payload);

            using var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

    }
}