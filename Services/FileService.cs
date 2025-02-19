using be_magang.Data;
using be_magang.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration.UserSecrets;
using System;
using System.IO;
using System.Threading.Tasks;

namespace be_magang.Services
{
    public interface IFileService
    {
        Task<string> UploadFile(IFormFile file, int userId, string category);
        Task<byte[]> DownloadFile(string fileName, string category);
    }

    public class FileService : IFileService
    {
        private readonly string _uploadPath;
        private readonly AppDbContext _context;

        public FileService(IWebHostEnvironment environment, AppDbContext context)
        {
            _uploadPath = Path.Combine(environment.WebRootPath, "uploads");
            _context = context;

            // Buat folder utama jika belum ada
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }

        public async Task<string> UploadFile(IFormFile file, int userId, string category)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File tidak valid!");

            // Tentukan folder berdasarkan kategori
            string categoryPath = Path.Combine(_uploadPath, category);
            if (!Directory.Exists(categoryPath))
            {
                Directory.CreateDirectory(categoryPath);
            }

            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
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
                FileName = file.FileName,
                FilePath = $"/uploads/{category}/{fileName}",
                FileType = file.ContentType,
                FileSize = file.Length,
                FileCategory = category,
                UploadDate = DateTime.UtcNow
            };

            _context.FileRecord.Add(uploadedFile);
            await _context.SaveChangesAsync();

            return uploadedFile.FilePath; // Kembalikan URL file yang telah diupload
        }

        public async Task<byte[]> DownloadFile(string fileName, string category)
        {
            string filePath = Path.Combine(_uploadPath, category, fileName);
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File tidak ditemukan");

            return await File.ReadAllBytesAsync(filePath);
        }
    }

}
