using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Services.TreeServices;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.Services;

namespace SupervisorMobility.API.Controllers.IS_Controllers
{
    [Route("api/IS/Template")]
    [ApiController]
    public class TemplateController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IStampingRepository _stampingRepository;
        private readonly IWebHostEnvironment _env;
        public TemplateController(IStampingRepository stampingRepository, IWebHostEnvironment env, IMapper mapper)
        {
            _stampingRepository = stampingRepository ??
                throw new ArgumentNullException(nameof(stampingRepository));
            _mapper = mapper ??
                  throw new ArgumentNullException(nameof(mapper));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }
    }
}
