using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PDFerter.Contracts;
using PDFerter.Core.Interfaces;
using PDFerter.Models;

namespace PDFerter.Controllers
{
    public class PdfController : Controller
    {

        private readonly ILogger<PdfController> _logger;

        private readonly IPDFerterService _pdfService;

        public PdfController(ILogger<PdfController> logger, IPDFerterService pdfService)
        {
            _logger = logger;
            _pdfService = pdfService;
        }

        [HttpPost(ApiRoutes.Convert)]
        public async Task<IActionResult> Convert(ICollection<IFormFile> files)
        {

            _logger.LogInformation("XD");
            var localFilePaths = await _pdfService.saveFilesLocally(files);
            var resultFile = await _pdfService.mergeTwoPDFs(localFilePaths);

            return Ok(new PDFileResult
            {
                Id = Guid.NewGuid(),
                File = resultFile,
            });
        }



    }
}