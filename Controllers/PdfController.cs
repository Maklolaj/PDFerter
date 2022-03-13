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


            return Ok("It`s okay");
        }



    }
}