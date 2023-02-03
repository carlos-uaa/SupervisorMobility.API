using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Models.FileUpload;
using System.Net;
using System.Text.RegularExpressions;

namespace SupervisorMobility.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public FileController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpPost]
        public async Task<ActionResult<UploadResult>> UploadFile(IFormFile file)
        {
            UploadResult uploadResult = new UploadResult();
            string trustedFileNameForFileStorage;
            var untrustedFileName = file.FileName;
            uploadResult.FileName = untrustedFileName;

            var trsutedFileNameForDisplay = WebUtility.HtmlEncode(untrustedFileName);


            Regex regexcsv = new Regex(".+\\.csv", RegexOptions.Compiled);
            Regex regexlsx = new Regex(".+\\.xlsx", RegexOptions.Compiled);

            if (regexcsv.IsMatch(untrustedFileName))
                trustedFileNameForFileStorage = Path.ChangeExtension(Path.GetRandomFileName(), "csv");
            else if (regexlsx.IsMatch(untrustedFileName))
                trustedFileNameForFileStorage = Path.ChangeExtension(Path.GetRandomFileName(), "xlsx");
            else
                trustedFileNameForFileStorage = Path.GetRandomFileName();

            //trustedFileNameForFileStorage = Path.GetRandomFileName();
            var path = Path.Combine(_env.ContentRootPath, "uploads", trustedFileNameForFileStorage);

            await using FileStream fs = new(path, FileMode.Create);
            await file.CopyToAsync(fs);

            uploadResult.StorageFileName= trustedFileNameForFileStorage;

            return Ok(uploadResult);
        }

    }
}
