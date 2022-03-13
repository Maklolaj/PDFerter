using System;
using PdfSharp.Pdf;

namespace PDFerter.Models
{
    public class PDFileResult
    {
        public Guid Id { get; set; }
        public PdfDocument File { get; set; }

    }
}