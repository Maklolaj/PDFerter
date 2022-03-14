using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PDFerter.Contracts;
using PDFerter.Core.Interfaces;

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
    }
}