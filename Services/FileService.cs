using be_magang.Data;
using be_magang.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace be_magang.Services
{
    public interface IFileService
    {
        Task<FileRecord> UploadFile(IFormFile file, int userId, string category);
        Task<byte[]> DownloadFile(string fileName, string category);
    }

    public class FileService : IFileService
    {
        private readonly string _uploadPath;
        private readonly AppDbContext _context;

        public FileService(IWebHostEnvironment environment, AppDbContext context)
        {
            _uploadPath = Path.Combine(environment.ContentRootPath, "uploads");
            _context = context;
            // Buat folder utama jika belum ada
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }

        public async Task<FileRecord> UploadFile(IFormFile file, int userId, string category)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File tidak valid!");

            // Tentukan folder berdasarkan kategori
            string categoryPath = Path.Combine(_uploadPath, category);
            if (!Directory.Exists(categoryPath))
            {
                Directory.CreateDirectory(categoryPath);
            }

            // Deteksi informasi file secara otomatis
            string originalFileName = file.FileName;
            string fileExtension = Path.GetExtension(originalFileName);
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(originalFileName);
            string detectedFileType = DetectFileType(file, fileExtension);

            // Generate nama file unik untuk penyimpanan
            string fileName = $"{Guid.NewGuid()}{fileExtension}";
            string filePath = Path.Combine(categoryPath, fileName);

            // Simpan file ke folder yang sesuai
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Simpan metadata file ke database
            var uploadedFile = new FileRecord
            {
                UserId = userId,
                FileName = originalFileName,
                FilePath = $"/uploads/{category}/{fileName}",
                FileType = file.ContentType,
                FileSize = file.Length,
                FileCategory = category,
                UploadDate = DateTime.UtcNow,
                FileExtension = fileExtension,
                DetectedFileType = detectedFileType,
                FileDescription = GenerateFileDescription(fileNameWithoutExt, detectedFileType, category)
            };

            _context.FileRecords.Add(uploadedFile);
            await _context.SaveChangesAsync();

            return uploadedFile; // Kembalikan objek file lengkap
        }

        public async Task<byte[]> DownloadFile(string fileName, string category)
        {
            string filePath = Path.Combine(_uploadPath, category, fileName);
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File tidak ditemukan");

            return await File.ReadAllBytesAsync(filePath);
        }

        // Method untuk mendeteksi tipe file berdasarkan konten dan ekstensi
        private string DetectFileType(IFormFile file, string extension)
        {
            // Kelompokkan berdasarkan ekstensi
            extension = extension.ToLowerInvariant();

            // Dokumen
            if (new[] { ".doc", ".docx", ".pdf", ".txt", ".rtf" }.Contains(extension))
                return "Dokumen";

            // Gambar
            if (new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".svg" }.Contains(extension))
                return "Gambar";

            // Spreadsheet
            if (new[] { ".xls", ".xlsx", ".csv" }.Contains(extension))
                return "Spreadsheet";

            // Presentasi
            if (new[] { ".ppt", ".pptx" }.Contains(extension))
                return "Presentasi";

            // Video
            if (new[] { ".mp4", ".avi", ".mov", ".wmv", ".mkv" }.Contains(extension))
                return "Video";

            // Audio
            if (new[] { ".mp3", ".wav", ".ogg", ".flac" }.Contains(extension))
                return "Audio";

            // Arsip
            if (new[] { ".zip", ".rar", ".7z", ".tar", ".gz" }.Contains(extension))
                return "Arsip";

            // Jika tidak terdeteksi
            return "Lainnya";
        }

        // Method untuk membuat deskripsi otomatis
        private string GenerateFileDescription(string fileName, string fileType, string category)
        {
            return $"{fileType} - {fileName} (kategori: {category})";
        }
    }
}