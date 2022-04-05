using PDFerter.Core.Interfaces;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using Microsoft.Extensions.Logging;
using ICSharpCode.SharpZipLib.Zip;
using PDFerter.Contracts;

namespace PDFerter.Core.Services
{
    public class PDFerterService : IPDFerterService
    {
        private readonly ILogger<PDFerterService> _logger;
        public PDFerterService(ILogger<PDFerterService> logger)
        {
            _logger = logger;
        }

        public async Task<byte[]> mergeTwoPDFs(IFormFile PdfFileOne, IFormFile PdfFileTwo)
        {
            PdfDocument document = new PdfDocument();

            foreach (var fileStream in new List<Stream>() { PdfFileOne.OpenReadStream(), PdfFileTwo.OpenReadStream() })
            {
                PdfDocument inputPDFDocument = await Task.Run(() => PdfReader.Open(fileStream, PdfDocumentOpenMode.Import));

                document.Version = inputPDFDocument.Version;

                foreach (PdfPage page in inputPDFDocument.Pages)
                {
                    document.AddPage(page);
                }
            }
            MemoryStream stream = new MemoryStream();
            document.Save(stream);
            byte[] docBytes = stream.ToArray();
            return docBytes;
        }

        public async Task<List<byte[]>> splitPDF(IFormFile file, int splitIndex)
        {
            PdfDocument inputPDFDocument = PdfReader.Open(file.OpenReadStream(), PdfDocumentOpenMode.Import);

            PdfDocument document1 = new PdfDocument();


            PdfDocument document2 = new PdfDocument();

            byte[] file1 = null;
            byte[] file2 = null;

            if (splitIndex <= inputPDFDocument.PageCount - 1 && splitIndex > 0)
            {
                for (int i = 0; i < splitIndex; i++)
                {
                    document1.AddPage(inputPDFDocument.Pages[i]);
                }
                using (MemoryStream stream1 = new MemoryStream())
                {
                    document1.Save(stream1, true);
                    file1 = stream1.ToArray();
                    stream1.Close();
                }



                for (int i = splitIndex; i <= inputPDFDocument.PageCount - 1; i++)
                {
                    document2.AddPage(inputPDFDocument.Pages[i]);
                }
                using (MemoryStream stream2 = new MemoryStream())
                {
                    document2.Save(stream2, true);
                    file2 = stream2.ToArray();
                    stream2.Close();
                }

                return new List<byte[]>() { file1, file2 };
            }
            return null;


        }

        public byte[] CreateZipResult(List<byte[]> result)
        {
            MemoryStream outputMemStream = new MemoryStream();

            using (ZipOutputStream zipOutputStream = new ZipOutputStream(outputMemStream))
            {
                zipOutputStream.SetLevel(9);

                byte[] buffer = new byte[4096];

                for (int i = 0; i < result.Count; i++)
                {
                    ZipEntry entry = new ZipEntry($"splitResult{i + 1}.pdf");
                    entry.DateTime = DateTime.Now;
                    entry.IsUnicodeText = true;
                    zipOutputStream.PutNextEntry(entry);

                    zipOutputStream.Write(result[i]);

                    zipOutputStream.CloseEntry();
                }
                zipOutputStream.Close();
            }
            byte[] byteArray = outputMemStream.ToArray();

            return byteArray;

        }
    }
}
