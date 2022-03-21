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

namespace PDFerter.Controllers
{
    public class PdfController : Controller
    {

        private readonly ILogger<PdfController> _logger;

        private readonly IPDFerterService _pdfService;

        private readonly IHostingEnvironment _oIHostingEnvironment;

        public PdfController(ILogger<PdfController> logger, IPDFerterService pdfService, IHostingEnvironment oIHostingEnvironment)
        {
            _logger = logger;
            _pdfService = pdfService;
            _oIHostingEnvironment = oIHostingEnvironment;
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
        public async Task<FileContentResult> test()
        {
            var file1 = File(System.IO.File.ReadAllBytes(@"C:/Users/mzele/Documents/Projects/PDFerter/WorkFiles/ResultFiles/splitResult1.pdf"),
            "application/octet-stream", "splitResult1.pdf");
            file1.FileDownloadName = "splitResult1.pdf";

            var file2 = File(System.IO.File.ReadAllBytes(@"C:/Users/mzele/Documents/Projects/PDFerter/WorkFiles/ResultFiles/splitResult2.pdf"),
            "application/octet-stream", "splitResult1.pdf");
            file2.FileDownloadName = "splitResult2.pdf";

            var result = new List<FileContentResult> { file1, file2 };

            using (ZipOutputStream zipOutputStream = new ZipOutputStream(System.IO.File.Create(@"C:/Users/mzele/Documents/Projects/PDFerter/WorkFiles/ResultFiles/MyZup.zip")))
            {
                zipOutputStream.SetLevel(9);

                byte[] buffer = new byte[4096];

                for (int i = 0; i < result.Count; i++)
                {
                    ZipEntry entry = new ZipEntry(result[i].FileDownloadName);
                    entry.DateTime = DateTime.Now;
                    entry.IsUnicodeText = true;
                    zipOutputStream.PutNextEntry(entry);

                    using (FileStream oFileStream = System.IO.File.OpenRead($@"C:/Users/mzele/Documents/Projects/PDFerter/WorkFiles/ResultFiles/splitResult{i + 1}.pdf"))
                    {
                        int sourceBytes;
                        do
                        {
                            sourceBytes = oFileStream.Read(buffer, 0, buffer.Length);
                            zipOutputStream.Write(buffer, 0, sourceBytes);
                        } while (sourceBytes > 0);
                    }

                }
                zipOutputStream.Finish();
                zipOutputStream.Flush();
                zipOutputStream.Close();


            }

            var finalResult = File(System.IO.File.ReadAllBytes(@"C:/Users/mzele/Documents/Projects/PDFerter/WorkFiles/ResultFiles/MyZup.zip"),
            "application/zip", "result.zip");

            if (System.IO.File.Exists(@"C:/Users/mzele/Documents/Projects/PDFerter/WorkFiles/ResultFiles/MyZup.zip"))
            {
                System.IO.File.Delete(@"C:/Users/mzele/Documents/Projects/PDFerter/WorkFiles/ResultFiles/MyZup.zip");
            }

            if (finalResult == null)
            {
                throw new Exception(String.Format("Nothing Found"));
            }

            return finalResult;
        }

    }
}