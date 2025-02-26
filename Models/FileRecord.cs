using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace be_magang.Models
{
    public class FileRecord
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }
        public string FileCategory { get; set; }
        public DateTime UploadDate { get; set; }

        // Tambahan properti untuk metadata
        public string FileDescription { get; set; }
        public string FileExtension { get; set; }
        public string DetectedFileType { get; set; } // Tipe file yang terdeteksi otomatis

        // Property untuk form upload
        [NotMapped]
        public IFormFile File { get; set; }
    }
}