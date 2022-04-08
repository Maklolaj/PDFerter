using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PDFerter.Contracts;
using PDFerter.Core.Interfaces;
using PDFerter.Models;

namespace PDFerter.Controllers
{
    public class PdfController : Controller
    {
        private readonly IPDFerterService _pdfService;
        public PdfController(ILogger<PdfController> logger, IPDFerterService pdfService)
        {
            _pdfService = pdfService;
        }

        [HttpPost(ApiRoutes.Merge)]
        public async Task<IActionResult> Merge(MergeFilesModel files)
        {
            if (!ModelState.IsValid) return BadRequest("Unsupported file format");

            var mergeResult = await _pdfService.mergeTwoPDFs(files.PdfFileOne, files.PdfFileTwo);
            if (mergeResult == null) return BadRequest("Bad Request");

            return File(mergeResult, "application/pdf", "test.pdf");
        }

        [HttpPost(ApiRoutes.Split)]
        public async Task<IActionResult> Split([FromRoute] string index, SplitFileModel file)
        {
            if (!ModelState.IsValid || !int.TryParse(index, out int number)) return BadRequest("Bad Request");

            var splitResult = await _pdfService.splitPDF(file.PdfFile, Int32.Parse(index));
            if (splitResult == null) return BadRequest("Bad Request");

            var zip = _pdfService.CreateZipResult(splitResult);
            var result = File(zip, "application/octet-stream", "result.zip");
            return result;
        }
    }
}
