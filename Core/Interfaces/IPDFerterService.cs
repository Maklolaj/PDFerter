using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using PdfSharp.Pdf;
using System.IO;

namespace PDFerter.Core.Interfaces
{
    public interface IPDFerterService
    {
        Task<byte[]> mergeTwoPDFs(IFormFile PdfFileOne, IFormFile PdfFileTwo);

        byte[] CreateZipResult(List<byte[]> result);

        Task<List<byte[]>> splitPDF(IFormFile pdfFile, int splitIndex);
    }

}