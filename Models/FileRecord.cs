using Microsoft.AspNetCore.SignalR;
using System.ComponentModel.DataAnnotations.Schema;

namespace be_magang.Models
{
    public class FileRecord
    {
        public int Id { get; set; }
        public string FileName { get; set; }  
        public string FilePath { get; set; }  
        public string FileType { get; set; } // Jenis file (image/png, application/pdf, dll)
        public long FileSize { get; set; } // Ukuran file (dalam byte)
        public DateTime UploadDate { get; set; } = DateTime.UtcNow;
        public string FileCategory { get; set; }

        [NotMapped]
        public IFormFile File { get; set; }
        public int UserId { get; set; }
    }

}
