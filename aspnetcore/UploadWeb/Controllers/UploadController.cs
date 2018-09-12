using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.IO;
using System.Threading.Tasks;

namespace UploadWeb.Controllers
{
    [Route("api/upload")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IHostingEnvironment _hostEnvironment;
        private Exception exx;

        public UploadController(IHostingEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;

            _hostEnvironment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\files");
        }

        [HttpPost, DisableRequestSizeLimit]
        public async Task<IActionResult> UploadFile()
        {
            if (!Request.HasFormContentType)
                return BadRequest();

            if (!Directory.Exists(_hostEnvironment.WebRootPath))
            {
                Directory.CreateDirectory(_hostEnvironment.WebRootPath);
            }

            var form = Request.Form;

            foreach (var file in form.Files)
            {
                using (var readStream = file.OpenReadStream())
                {
                    var filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Value.Trim('"');

                    filename = _hostEnvironment.WebRootPath + $@"\{filename}";

                    using (FileStream fs = System.IO.File.Create(filename))
                    {
                        await file.CopyToAsync(fs);
                        await fs.FlushAsync();
                    }

                }
            }
            return Ok();
        }
    }
}