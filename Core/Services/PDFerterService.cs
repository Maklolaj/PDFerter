using PDFerter.Core.Interfaces;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using PdfSharp.Pdf;
using System.Linq;
using PdfSharp.Pdf.IO;
using Microsoft.Extensions.Logging;

namespace PDFerter.Core.Services
{
    public class PDFerterService : IPDFerterService
    {
        private readonly ILogger<PDFerterService> _logger;
        public PDFerterService(ILogger<PDFerterService> logger)
        {
            _logger = logger;
        }
        public async Task<string[]> saveFilesLocally(ICollection<IFormFile> files)
        {
            List<string> filepaths = new List<string>();

            foreach (IFormFile file in files)
            {
                filepaths.Add(await performSaveFile(file));
            }
            return filepaths.ToArray();
        }

        public async Task<PdfDocument> mergeTwoPDFs(string[] pdfFilePaths)
        {
            PdfDocument document = new PdfDocument();

            foreach (var filePath in pdfFilePaths)
            {
                PdfDocument inputPDFDocument = await Task.Run(() => PdfReader.Open(filePath, PdfDocumentOpenMode.Import));

                document.Version = inputPDFDocument.Version;

                foreach (PdfPage page in inputPDFDocument.Pages)
                {
                    document.AddPage(page);
                }
            }

            foreach (var filePath in pdfFilePaths)
            {
                performDeleteFile(filePath);
            }

            document.Save(@"C:/Users/mzele/Documents/Projects/PDFerter/WorkFiles/ResultFiles/result.pdf");

            return PdfReader.Open(@"C:/Users/mzele/Documents/Projects/PDFerter/WorkFiles/ResultFiles/result.pdf", PdfDocumentOpenMode.Import);
        }

        public void performDeleteFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    _logger.LogInformation("File deleted.");
                }
                else
                {
                    _logger.LogInformation("File not found.");
                }
            }
            catch (IOException ioExp)
            {
                _logger.LogInformation(ioExp.Message);
            }
        }

        public async Task<string> performSaveFile(IFormFile file)
        {
            var id = Guid.NewGuid();
            var filePath = @$"C:/Users/mzele/Documents/Projects/PDFerter/WorkFiles/file{id}.pdf";
            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                    fileStream.Close();
                    _logger.LogInformation($"file{id}.pdf Saved.");
                }

            }
            catch (IOException ioExp)
            {
                _logger.LogInformation(ioExp.Message);
            }

            return filePath;
        }
    }

}
