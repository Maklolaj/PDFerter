using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PDFerter.Contracts;

namespace PDFerter.Controllers
{
    public class PdfController : Controller
    {

        private readonly ILogger<PdfController> _logger;

        public PdfController(ILogger<PdfController> logger)
        {
            _logger = logger;
        }

        [HttpPost(ApiRoutes.Convert)]
        public async Task<IActionResult> Convert(ICollection<IFormFile> files)
        {
            var test = files;

            foreach (IFormFile file in files)
            {
                using (var fileStream = new FileStream(@$"C:/Users/mzele/Documents/Projects/PDFerter/TestFiles/file{Guid.NewGuid()}.pdf", FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                    fileStream.Close();
                }
            }


            return Ok("It`s okay");
        }



    }
}