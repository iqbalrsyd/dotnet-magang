using be_magang.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using be_magang.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace be_magang.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly IUserService _userService;

        public FileController(IFileService fileService, IUserService userService)
        {
            _fileService = fileService;
            _userService = userService;
        }

        // Upload file dengan kategori (profile/general)
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadFile([FromForm] FileRecord model, [FromQuery] string category)
        {
            if (model == null || model.File == null || model.File.Length == 0)
                return BadRequest(new { message = "File tidak ditemukan atau kosong!" });

            try
            {
                var userId = _userService.GetUserId(); 

                var fileName = await _fileService.UploadFile(model.File, userId, category);
                return Ok(new { message = "File berhasil diupload", fileName });
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
                return File(fileBytes, "application/octet-stream", fileName);
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
