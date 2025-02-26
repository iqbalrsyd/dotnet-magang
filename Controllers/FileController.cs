using be_magang.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using be_magang.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using be_magang.Data;

namespace be_magang.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly IUserService _userService;
        private readonly AppDbContext _context;

        public FileController(IFileService fileService, IUserService userService)
        {
            _fileService = fileService;
            _userService = userService;
        }

        // Upload file dengan kategori (profile/general)
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadFile(IFormFile file, [FromQuery] string category)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "File tidak ditemukan atau kosong!" });

            try
            {
                var userId = _userService.GetUserId();
                var uploadedFile = await _fileService.UploadFile(file, userId, category);

                return Ok(new
                {
                    message = "File berhasil diupload",
                    fileData = new
                    {
                        id = uploadedFile.Id,
                        fileName = uploadedFile.FileName,
                        filePath = uploadedFile.FilePath,
                        fileType = uploadedFile.FileType,
                        fileSize = uploadedFile.FileSize,
                        fileCategory = uploadedFile.FileCategory,
                        uploadDate = uploadedFile.UploadDate,
                        detectedFileType = uploadedFile.DetectedFileType,
                        fileExtension = uploadedFile.FileExtension,
                        fileDescription = uploadedFile.FileDescription
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Terjadi kesalahan saat upload", error = ex.Message });
            }
        }

        // Download file berdasarkan kategori
        [HttpGet("download/{category}/{fileName}")]
        public async Task<IActionResult> DownloadFile(string category, string fileName)
        {
            try
            {
                var fileBytes = await _fileService.DownloadFile(fileName, category);

                // Cari informasi file dari database untuk nama asli
                var fileRecord = _context.FileRecords
                    .FirstOrDefault(f => f.FilePath.EndsWith($"{category}/{fileName}"));

                string downloadName = fileRecord?.FileName ?? fileName;
                string contentType = fileRecord?.FileType ?? "application/octet-stream";

                return File(fileBytes, contentType, downloadName);
            }
            catch (FileNotFoundException)
            {
                return NotFound(new { message = "File tidak ditemukan!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}