using System.Collections.Generic;
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
        private readonly IPDFerterService _pdfService;
        public PdfController(ILogger<PdfController> logger, IPDFerterService pdfService)
        {
            _pdfService = pdfService;
        }

        [HttpPost(ApiRoutes.Merge)]
        public async Task<FileContentResult> Merge(ICollection<IFormFile> files)
        {

            if (ModelState.IsValid)  // ICollection<InputFileModel> DOES NOT WORK // CREATE CUSTOM VALIDATION
            {
                await _pdfService.mergeTwoPDFs(await _pdfService.saveFilesLocally(files));

                var realResultFile = File(await System.IO.File.ReadAllBytesAsync(@$"{LocalPaths.resultFilesPath}result.pdf"),
                    "application/octet-stream", "result.pdf");

                _pdfService.performDeleteFile(@$"{LocalPaths.resultFilesPath}result.pdf");
                return realResultFile;
            }
            return new FileContentResult(new byte[0], "empty");
        }

        [HttpPost(ApiRoutes.Split)]
        public async Task<FileContentResult> Split([FromRoute] int index, InputFileModel file)
        {
            if (ModelState.IsValid)
            {
                await _pdfService.splitTwoPDFs(await _pdfService.performSaveFile(file.PdfFile), index);
                var finalResult = File(await _pdfService.CreateZipResult(), "application/zip", "result.zip");
                return finalResult;
            }
            return new FileContentResult(new byte[0], "empty");

        }
    }
}
