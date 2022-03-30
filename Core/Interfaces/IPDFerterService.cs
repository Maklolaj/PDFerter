using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using PdfSharp.Pdf;
using System.IO;

namespace PDFerter.Core.Interfaces
{
    public interface IPDFerterService
    {
        Task<string[]> saveFilesLocally(ICollection<IFormFile> files);

        Task<PdfDocument> mergeTwoPDFs(string[] pdfFilePaths);

        void performDeleteFile(string filePath);

        Task<string> performSaveFile(IFormFile filePath);

        Task<bool> splitTwoPDFs(string pdfFilePath, int splitIndex);

        Task<byte[]> CreateZipResult();

        Task<List<byte[]>> testSplit(IFormFile pdfFile, int splitIndex);
    }

}