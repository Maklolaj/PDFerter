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
            if (ModelState.IsValid)
            {
                var mergeResult = await _pdfService.mergeTwoPDFs(files.PdfFileOne, files.PdfFileTwo);
                if (mergeResult != null)
                {
                    return File(mergeResult, "application/pdf", "test.pdf");
                }
                return BadRequest("Bad Request");
            }

            return BadRequest("Unsupported file format");
        }

        [HttpPost(ApiRoutes.Split)]
        public async Task<IActionResult> Split([FromRoute] string index, SplitFileModel file)
        {
            if (ModelState.IsValid && int.TryParse(index, out int number))
            {
                var splitResult = await _pdfService.splitPDF(file.PdfFile, Int32.Parse(index));
                if (splitResult != null)
                {
                    var zip = _pdfService.CreateZipResult(splitResult);
                    var result = File(zip, "application/octet-stream", "result.zip");
                    return result;
                }
                return BadRequest("Bad Request");
            }
            return BadRequest("Bad Request");
        }
    }
}
