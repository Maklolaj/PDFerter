using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PDFerter.Contracts;
using PDFerter.Core.Interfaces;
using System.IO;
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
            var realResultFile = File(System.IO.File.ReadAllBytes(@$"{LocalPaths.resultFilesPath}result.pdf"),
                "application/octet-stream", "result.pdf");

            _pdfService.performDeleteFile(@$"{LocalPaths.resultFilesPath}result.pdf");
            return realResultFile;
        }

        [HttpPost(ApiRoutes.Split)]
        public async Task<FileContentResult> Split([FromRoute] int index)
        {
            _logger.LogInformation(index.ToString());
            _pdfService.splitTwoPDFs(@"C:/Users/mzele/Documents/Projects/PDF converter test/inputFiles/ABCD file.pdf", index); // FIX ASYNC BUG! 
            var finalResult = File(_pdfService.CreateZipResult(), "application/zip", "result.zip"); // FIX ASYNC BUG! 
            return finalResult;
        }

        [HttpPost("api/test")]
        public IActionResult test()
        {

            //var file1 = System.IO.File.ReadAllBytes(@"PDFerter/WorkFiles/ResultFiles/splitResult1.pdf");
            var localPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"..\..\..\WorkFiles\Resultfiles"));
            _logger.LogInformation(localPath);

            var resultFilesPath = Path.GetFullPath(Path.Combine(localPath, @"WorkFiles\Resultfiles"));
            _logger.LogInformation(resultFilesPath);


            return Ok();
        }

    }
}