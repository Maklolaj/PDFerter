using System.Collections.Generic;
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
                await _pdfService.mergeTwoPDFs(await _pdfService.saveFilesLocally(new List<IFormFile>() { files.PdfFileOne, files.PdfFileTwo }));

                var realResultFile = File(await System.IO.File.ReadAllBytesAsync(@$"{LocalPaths.resultFilesPath}result.pdf"),
                    "application/octet-stream", "result.pdf");

                _pdfService.performDeleteFile(@$"{LocalPaths.resultFilesPath}result.pdf");
                return realResultFile;
            }

            return BadRequest("Unsupported file format");
        }

        [HttpPost(ApiRoutes.Split)]
        public async Task<IActionResult> Split([FromRoute] string index, SplitFileModel file)
        {
            if (ModelState.IsValid && int.TryParse(index, out int number))
            {
                if (await _pdfService.splitTwoPDFs(await _pdfService.performSaveFile(file.PdfFile), number))
                {
                    var finalResult = File(await _pdfService.CreateZipResult(), "application/zip", "result.zip");
                    return finalResult;
                }
                return BadRequest("Invalid index");
            }

            return BadRequest("Unsupported file format");
        }
    }
}
