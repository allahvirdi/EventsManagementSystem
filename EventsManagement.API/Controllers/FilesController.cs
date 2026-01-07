using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using EventsManagement.Shared.DTOs;
using EventsManagement.Infrastructure.Data;
using EventsManagement.API.Services;
using Serilog;

namespace EventsManagement.API.Controllers
{
    /// <summary>
    /// کنترلر مدیریت فایل‌ها و اسناد
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FilesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileUploadService _fileUploadService;

        public FilesController(ApplicationDbContext context, IFileUploadService fileUploadService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
        }

        /// <summary>
        /// آپلود فایل
        /// POST: api/Files/Upload
        /// </summary>
        [HttpPost("Upload")]
        [Authorize(Roles = "Admin,Manager,Operator")]
        [RequestSizeLimit(104857600)] // 100MB
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ApiResult<object>>> UploadFile([FromForm] FileUploadRequest request)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                
                if (userId == null)
                    return Ok(ApiResult<object>.Failure("کاربر احراز هویت نشده است", 401));

                var result = await _fileUploadService.UploadFileAsync(
                    request.File, 
                    request.EventId, 
                    request.TaskId, 
                    userId, 
                    request.Description ?? "");

                if (!result.Success)
                    return Ok(ApiResult<object>.Failure(result.Message, 400));

                var fileInfo = new
                {
                    result.Metadata?.Id,
                    result.Metadata?.FileName,
                    result.Metadata?.FileType,
                    result.Metadata?.FileSize,
                    UploadedAt = result.Metadata?.CreatedAt
                };

                return Ok(ApiResult<object>.Success(fileInfo, result.Message));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "خطا در آپلود فایل");
                return Ok(ApiResult<object>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// دریافت فایل‌های یک رویداد
        /// GET: api/Files/ByEvent/{eventId}
        /// </summary>
        [HttpGet("ByEvent/{eventId}")]
        public async Task<ActionResult<ApiResult<List<object>>>> GetEventFiles(int eventId)
        {
            try
            {
                var files = await _context.DocumentMetadatas
                    .Where(d => d.EventId == eventId && !d.IsDeleted)
                    .OrderByDescending(d => d.CreatedAt)
                    .Select(d => new
                    {
                        d.Id,
                        d.FileName,
                        d.FileType,
                        d.FileSize,
                        d.Description,
                        d.UploadedBy,
                        UploadedAt = d.CreatedAt
                    })
                    .ToListAsync();

                return Ok(ApiResult<List<object>>.Success(files.Cast<object>().ToList(), $"{files.Count} فایل یافت شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در دریافت فایل‌های رویداد {eventId}");
                return Ok(ApiResult<List<object>>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// دریافت فایل‌های یک وظیفه
        /// GET: api/Files/ByTask/{taskId}
        /// </summary>
        [HttpGet("ByTask/{taskId}")]
        public async Task<ActionResult<ApiResult<List<object>>>> GetTaskFiles(int taskId)
        {
            try
            {
                var files = await _context.DocumentMetadatas
                    .Where(d => d.TaskId == taskId && !d.IsDeleted)
                    .OrderByDescending(d => d.CreatedAt)
                    .Select(d => new
                    {
                        d.Id,
                        d.FileName,
                        d.FileType,
                        d.FileSize,
                        d.Description,
                        d.UploadedBy,
                        UploadedAt = d.CreatedAt
                    })
                    .ToListAsync();

                return Ok(ApiResult<List<object>>.Success(files.Cast<object>().ToList(), $"{files.Count} فایل یافت شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در دریافت فایل‌های وظیفه {taskId}");
                return Ok(ApiResult<List<object>>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// دانلود فایل
        /// GET: api/Files/Download/{id}
        /// </summary>
        [HttpGet("Download/{id}")]
        public async Task<IActionResult> DownloadFile(int id)
        {
            try
            {
                var result = await _fileUploadService.GetFilePathAsync(id);
                if (!result.Success || result.FilePath == null)
                    return NotFound(new { message = result.Message });

                var document = await _context.DocumentMetadatas.FindAsync(id);
                if (document == null)
                    return NotFound(new { message = "فایل یافت نشد" });

                var fileBytes = await System.IO.File.ReadAllBytesAsync(result.FilePath);
                return File(fileBytes, "application/octet-stream", document.FileName);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در دانلود فایل {id}");
                return StatusCode(500, new { message = "خطای سیستمی رخ داده است" });
            }
        }

        /// <summary>
        /// حذف فایل
        /// DELETE: api/Files/{id}
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ApiResult>> DeleteFile(int id)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                
                if (userId == null)
                    return Ok(ApiResult.Failure("کاربر احراز هویت نشده است", 401));

                var result = await _fileUploadService.DeleteFileAsync(id, userId);

                if (!result.Success)
                    return Ok(ApiResult.Failure(result.Message, 404));

                return Ok(ApiResult.Success(result.Message));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در حذف فایل {id}");
                return Ok(ApiResult.Failure("خطای سیستمی رخ داده است", 500));
            }
        }
    }

    /// <summary>
    /// درخواست آپلود فایل
    /// </summary>
    public class FileUploadRequest
    {
        /// <summary>
        /// فایل برای آپلود
        /// </summary>
        public required IFormFile File { get; set; }

        /// <summary>
        /// شناسه رویداد (اختیاری)
        /// </summary>
        public int? EventId { get; set; }

        /// <summary>
        /// شناسه وظیفه (اختیاری)
        /// </summary>
        public int? TaskId { get; set; }

        /// <summary>
        /// توضیحات (اختیاری)
        /// </summary>
        public string? Description { get; set; }
    }
}
