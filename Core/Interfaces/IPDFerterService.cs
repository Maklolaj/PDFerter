using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using PdfSharp.Pdf;

namespace PDFerter.Core.Interfaces
{
    public interface IPDFerterService
    {
        Task<string[]> saveFilesLocally(ICollection<IFormFile> files);

        Task<PdfDocument> mergeTwoPDFs(string[] pdfFilePaths);

        void performDeleteFile(string filePath);

        Task<string> performSaveFile(IFormFile filePath);
    }

}