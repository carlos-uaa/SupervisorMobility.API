using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace SupervisorMobility.API.Controllers.SOS_Controllers
{
    [Route("api/Analysis_Process/Analysis")]
    [ApiController]
    public class AnalysisController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        public AnalysisController(IWebHostEnvironment env, IMapper mapper)
        {

            _mapper = mapper ??
                  throw new ArgumentNullException(nameof(mapper));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }
    }
}
