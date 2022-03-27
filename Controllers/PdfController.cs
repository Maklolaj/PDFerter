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
        private readonly IPDFerterService _pdfService;
        public PdfController(ILogger<PdfController> logger, IPDFerterService pdfService)
        {
            _pdfService = pdfService;
        }

        [HttpPost(ApiRoutes.Convert)]
        public async Task<FileContentResult> Convert(ICollection<IFormFile> files)
        {
            await _pdfService.mergeTwoPDFs(await _pdfService.saveFilesLocally(files));
            var realResultFile = File(await System.IO.File.ReadAllBytesAsync(@$"{LocalPaths.resultFilesPath}result.pdf"),
                "application/octet-stream", "result.pdf");

            _pdfService.performDeleteFile(@$"{LocalPaths.resultFilesPath}result.pdf");
            return realResultFile;
        }

        [HttpPost(ApiRoutes.Split)]
        public async Task<FileContentResult> Split([FromRoute] int index, IFormFile file)
        {
            await _pdfService.splitTwoPDFs(await _pdfService.performSaveFile(file), index);
            var finalResult = File(await _pdfService.CreateZipResult(), "application/zip", "result.zip");
            return finalResult;
        }
    }
}
