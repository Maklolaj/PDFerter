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
        public async Task<IActionResult> Split(int index)
        {
            _logger.LogInformation(index.ToString());
            _pdfService.splitTwoPDFs(@"C:/Users/mzele/Documents/Projects/PDF converter test/inputFiles/ABCD file.pdf", 2);

            return Ok("works?");
        }

        [HttpPost("api/test")]
        public async Task<ZipArchive> test()
        {
            var file1 = File(System.IO.File.ReadAllBytes(@"C:/Users/mzele/Documents/Projects/PDFerter/WorkFiles/ResultFiles/splitResult1.pdf"),
            "application/octet-stream", "splitResult1.pdf");

            var file2 = File(System.IO.File.ReadAllBytes(@"C:/Users/mzele/Documents/Projects/PDFerter/WorkFiles/ResultFiles/splitResult2.pdf"),
            "application/octet-stream", "splitResult1.pdf");

            var result = new List<FileContentResult> { file1, file2 };

            // using (var memoryStream = new MemoryStream())
            // {
            // using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            // {
            //     var file1 = archive.CreateEntry("file1.txt");
            //     using (var streamWriter = new StreamWriter(file1.Open()))
            //     {
            //         streamWriter.Write("content1");
            //     }

            //     var file2 = archive.CreateEntry("file2.txt");
            //     using (var streamWriter = new StreamWriter(file2.Open()))
            //     {
            //         streamWriter.Write("content2");
            //     }
            // }

            // return File(memoryStream.ToArray(), "application/zip", "Images.zip");
            if (System.IO.File.Exists("archive.zip"))
                System.IO.File.Delete("archive.zip");

            using var archive = ZipFile.Open(@"archive.zip", ZipArchiveMode.Create);

            var file11 = System.IO.File.OpenRead(@"C:/Users/mzele/Documents/Projects/PDFerter/WorkFiles/ResultFiles/splitResult1.pdf");
            var file22 = System.IO.File.OpenRead(@"C:/Users/mzele/Documents/Projects/PDFerter/WorkFiles/ResultFiles/splitResult2.pdf");

            var entry1 = archive.CreateEntry("test1.pdf", CompressionLevel.Optimal);
            // var entry2 = archive.CreateEntry("test2.pdf", CompressionLevel.Optimal);

            await file11.CopyToAsync(entry1.Open());

            await file22.CopyToAsync(entry1.Open());

            return archive;
        }

    }
}