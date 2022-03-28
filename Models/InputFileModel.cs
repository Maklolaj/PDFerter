using Microsoft.AspNetCore.Http;
using PDFerter.Validators;

namespace PDFerter.Models
{
    public class InputFileModel
    {
        [MaxFileSize(5 * 1024 * 1024)]
        [AllowedExtensions(new string[] { ".pdf" })]
        public IFormFile PdfFile { get; set; }
    }
}