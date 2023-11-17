using Microsoft.AspNetCore.Mvc;
using webapifoto.Models;

namespace webapifoto.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FotoController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FotoController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost]
        public async Task<ActionResult<Foto>> PostIndex([FromForm] Foto foto)
        {
            try
            {
                string webRootPath = _webHostEnvironment.WebRootPath;
                string rutaArchvos = Path.Combine(webRootPath, "files");

                if(foto.Archivo.Length > 0)
                {
                   if(!Directory.Exists(rutaArchvos))
                   {
                       Directory.CreateDirectory(rutaArchvos);
                   }
                   using(FileStream fileStream = System.IO.File.Create(Path.Combine(rutaArchvos, foto.Archivo.FileName)))
                   {
                       await foto.Archivo.CopyToAsync(fileStream);
                        fileStream.Flush();
                   }
                   foto.Url = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/files/" + foto.Archivo.FileName;
                }
            }
            catch(Exception ex)
            {
                return Problem(ex.Message + _webHostEnvironment.WebRootPath);
            }
            return CreatedAtAction(nameof(PostIndex), new { foto.Nombre, foto.Url });

        }
    }
}
