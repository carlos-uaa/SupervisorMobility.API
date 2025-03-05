using SupervisorMobility.API.DataAccess.Services.BackgroundProcessServices;

namespace SupervisorMobility.API.DataAccess.Services
{
    public class BackgroundProcessingService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ProcessHeadCountService _headcountService;
        private readonly ProcessWorkLoadService _workLoadService;

        public string _fileName = string.Empty;
        public int _userId = 0;
        public int _plantId = 0;
        public int _option = 0;

        public BackgroundProcessingService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _headcountService = new ProcessHeadCountService(serviceProvider);
            _workLoadService = new ProcessWorkLoadService(serviceProvider);
        }

        public async Task StartAsync(string fileName, int userId, int option, CancellationToken stoppingToken, int plantname = 0)
        {
            _fileName = fileName;
            _userId = userId;
            _option = option;
            _plantId = plantname;
            await StartAsync(CancellationToken.None);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            switch (_option)
            {
                case 1:
                    await _headcountService.ProcessDocumentHeadCountAsync(_fileName, _userId, stoppingToken);
                    break;
                case 2:
                    await _workLoadService.ProcessWorkLoadDataAsync(_fileName, _plantId, _userId, stoppingToken);
                    break;
            }

        }


    }//end background service
}
