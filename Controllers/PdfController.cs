using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PDFerter.Contracts;
using PDFerter.Core.Interfaces;
using System.IO.Compression;
using System.IO;
using System;
using Microsoft.AspNetCore.Hosting;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System.Reflection;

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
        public async Task<FileContentResult> Convert(ICollection<IFormFile> files)
        {
            await _pdfService.mergeTwoPDFs(await _pdfService.saveFilesLocally(files));
            var realResultFile = File(System.IO.File.ReadAllBytes(@"C:/Users/mzele/Documents/Projects/PDFerter/WorkFiles/ResultFiles/result.pdf"),
                "application/octet-stream", "result.pdf");

            _pdfService.performDeleteFile(@"C:/Users/mzele/Documents/Projects/PDFerter/WorkFiles/ResultFiles/result.pdf");
            return realResultFile;
        }

        [HttpPost(ApiRoutes.Split)]
        public async Task<FileContentResult> Split([FromRoute] int index)
        {
            _logger.LogInformation(index.ToString());
            _pdfService.splitTwoPDFs(@"C:/Users/mzele/Documents/Projects/PDF converter test/inputFiles/ABCD file.pdf", index);
            var finalResult = File(_pdfService.CreateZipResult(), "application/zip", "result.zip");
            return finalResult;
        }

        [HttpPost("api/test")]
        public IActionResult test()
        {


            return Ok();
        }

    }
}