namespace SupervisorMobility.API
{
    public class CustomHttpClientService
    {
        private readonly HttpClient _apiLoginClient;
        private readonly HttpClient _apiAppClient;
        private readonly HttpClient _apiExtendsAppClient;
        private readonly HttpClient _bridgeHttpClient;

        public CustomHttpClientService(IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //development
                _apiLoginClient = new HttpClient { BaseAddress = new Uri("http://10.91.49.9:4251/") };
                _bridgeHttpClient = new HttpClient { BaseAddress = new Uri("https://10.91.49.2:3000/") };
                _apiAppClient = new HttpClient { BaseAddress = new Uri("https://localhost:10201/api/") };
                _apiExtendsAppClient = new HttpClient { BaseAddress = new Uri("https://localhost:10201/api/") };

            }
            else
            {
                //Production
                _apiLoginClient = new HttpClient { BaseAddress = new Uri("http://10.91.116.212:4251") };
                _bridgeHttpClient = new HttpClient { BaseAddress = new Uri("https://10.91.117.5:3000/") };
                _apiAppClient = new HttpClient { BaseAddress = new Uri("https://10.91.117.12:10201/api/") };
                _apiExtendsAppClient = new HttpClient { BaseAddress = new Uri("https://10.91.117.12:10207/api/") };
            }


        }

        public HttpClient GetLoginHttpClient()
        {
            return _apiLoginClient;
        }

        public HttpClient GetApiHttpClient()
        {
            return _apiAppClient;
        }
        public HttpClient GetApiExtendsHttpClient()
        {
            return _apiExtendsAppClient;
        }

        public HttpClient GetBridgeHttpClient()
        {
            return _bridgeHttpClient;
        }
    }
}
