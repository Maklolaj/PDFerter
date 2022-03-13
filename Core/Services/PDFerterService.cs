using PDFerter.Core.Interfaces;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using PdfSharp.Pdf;
using System.Linq;
using PdfSharp.Pdf.IO;

namespace PDFerter.Core.Services
{
    public class PDFerterService : IPDFerterService
    {
        public async Task<string[]> saveFilesLocally(ICollection<IFormFile> files)
        {
            string[] filepaths = new string[files.Count];

            foreach (IFormFile file in files)
            {
                var filePath = @$"C:/Users/mzele/Documents/Projects/PDFerter/TestFiles/file{Guid.NewGuid()}.pdf";
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                    fileStream.Close();
                    filepaths.Append(filePath);
                }
            }
            return filepaths;
        }

        public PdfDocument mergeTwoPDFs(string[] pdfFilePaths)
        {
            PdfDocument document = new PdfDocument();

            foreach (var filePath in pdfFilePaths)
            {

                PdfDocument inputPDFDocument = PdfReader.Open(filePath, PdfDocumentOpenMode.Import);

                document.Version = inputPDFDocument.Version;

                foreach (PdfPage page in inputPDFDocument.Pages)
                {
                    document.AddPage(page);
                }
            }

            document.Save(@"C:/Users/mzele/Documents/Projects/PDFerter/TestFiles/ResultFiles/result.pdf");

            return PdfReader.Open(@"C:/Users/mzele/Documents/Projects/PDFerter/TestFiles/ResultFiles/result.pdf", PdfDocumentOpenMode.Import);
        }
    }

}