using Microsoft.AspNetCore.Http;
using PDFerter.Validators;

namespace PDFerter.Models
{
    public class SplitFileModel
    {
        [MaxFileSize(5 * 1024 * 1024)]
        [AllowedExtensions(new string[] { ".pdf" })]
        public IFormFile PdfFile { get; set; }
    }

    public class MergeFilesModel
    {
        [MaxFileSize(5 * 1024 * 1024)]
        [AllowedExtensions(new string[] { ".pdf" })]
        public IFormFile PdfFileOne { get; set; }

        [MaxFileSize(5 * 1024 * 1024)]
        [AllowedExtensions(new string[] { ".pdf" })]
        public IFormFile PdfFileTwo { get; set; }
    }
}