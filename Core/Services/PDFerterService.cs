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

            document.Save(@$"{LocalPaths.resultFilesPath}result.pdf");

            return PdfReader.Open(@$"{LocalPaths.resultFilesPath}result.pdf", PdfDocumentOpenMode.Import);
        }

        public async Task<bool> splitTwoPDFs(string pdfFilePath, int splitIndex)
        {
            PdfDocument inputPDFDocument = await Task.Run(() => PdfReader.Open(pdfFilePath, PdfDocumentOpenMode.Import));

            PdfDocument document1 = new PdfDocument();
            document1.Version = inputPDFDocument.Version;

            PdfDocument document2 = new PdfDocument();
            document2.Version = inputPDFDocument.Version;

            if (splitIndex <= inputPDFDocument.PageCount - 1 && splitIndex > 0)
            {
                for (int i = 0; i < splitIndex; i++)
                {
                    document1.AddPage(inputPDFDocument.Pages[i]);
                }
                document1.Save(@$"{LocalPaths.resultFilesPath}splitResult1.pdf");

                for (int i = splitIndex; i <= inputPDFDocument.PageCount - 1; i++)
                {
                    document2.AddPage(inputPDFDocument.Pages[i]);
                }
                document2.Save(@$"{LocalPaths.resultFilesPath}splitResult2.pdf");

                performDeleteFile(pdfFilePath);
                return true;
            }
            return false;
        }

        public async Task<byte[]> CreateZipResult()
        {
            var file1 = await System.IO.File.ReadAllBytesAsync(@$"{LocalPaths.resultFilesPath}splitResult1.pdf");
            var file2 = await System.IO.File.ReadAllBytesAsync(@$"{LocalPaths.resultFilesPath}splitResult2.pdf");
            var result = new List<byte[]> { file1, file2 };

            using (ZipOutputStream zipOutputStream = new ZipOutputStream(System.IO.File.Create(@$"{LocalPaths.resultFilesPath}MyZup.zip")))
            {
                zipOutputStream.SetLevel(9);

                byte[] buffer = new byte[4096];

                for (int i = 0; i < result.Count; i++)
                {
                    ZipEntry entry = new ZipEntry($"splitResult{i + 1}.pdf");
                    entry.DateTime = DateTime.Now;
                    entry.IsUnicodeText = true;
                    zipOutputStream.PutNextEntry(entry);

                    using (FileStream oFileStream = System.IO.File.OpenRead($@"{LocalPaths.resultFilesPath}splitResult{i + 1}.pdf"))
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

            performDeleteFile(@$"{LocalPaths.resultFilesPath}splitResult1.pdf");
            performDeleteFile(@$"{LocalPaths.resultFilesPath}splitResult2.pdf");
            var finalResult = System.IO.File.ReadAllBytes(@$"{LocalPaths.resultFilesPath}MyZup.zip");

            if (System.IO.File.Exists(@$"{LocalPaths.resultFilesPath}MyZup.zip"))
            {
                System.IO.File.Delete(@$"{LocalPaths.resultFilesPath}MyZup.zip");
            }

            if (finalResult == null)
            {
                throw new Exception(String.Format("Nothing Found"));
            }

            return finalResult;
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
            var filePath = @$"{LocalPaths.workFilesPath}/file{id}.pdf";
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
        public async Task<List<byte[]>> testSplit(IFormFile stream, int splitIndex)
        {
            var pdfFile = stream.OpenReadStream();
            PdfDocument inputPDFDocument = PdfReader.Open(pdfFile, PdfDocumentOpenMode.Import);

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
                    file1 = stream1.ToArray(); ;
                }



                for (int i = splitIndex; i <= inputPDFDocument.PageCount - 1; i++)
                {
                    document2.AddPage(inputPDFDocument.Pages[i]);
                }
                using (MemoryStream stream2 = new MemoryStream())
                {
                    document1.Save(stream2, true);
                    file2 = stream2.ToArray(); ;
                }
            }

            pdfFile.Close();
            return new List<byte[]>() { file1, file2 };
        }


    }

}
