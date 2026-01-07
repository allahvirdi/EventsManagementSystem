using EventsManagement.Infrastructure.Data;
using EventsManagement.Shared.Entities;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace EventsManagement.API.Services
{
    /// <summary>
    /// سرویس مدیریت آپلود فایل
    /// </summary>
    public interface IFileUploadService
    {
        Task<(bool Success, string Message, DocumentMetadata? Metadata)> UploadFileAsync(
            IFormFile file, 
            int? eventId, 
            int? taskId, 
            string uploadedBy, 
            string description);
        
        Task<(bool Success, string Message)> DeleteFileAsync(int documentId, string deletedBy);
        
        Task<(bool Success, string Message, string? FilePath)> GetFilePathAsync(int documentId);
    }

    public class FileUploadService : IFileUploadService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        // محدودیت‌ها
        private readonly long _maxFileSizeBytes;
        private readonly string[] _allowedExtensions;
        private readonly string _uploadBasePath;

        public FileUploadService(
            ApplicationDbContext context, 
            IWebHostEnvironment environment,
            IConfiguration configuration)
        {
            _context = context;
            _environment = environment;
            _configuration = configuration;

            // خواندن تنظیمات از appsettings
            _maxFileSizeBytes = _configuration.GetValue<long>("FileUpload:MaxFileSizeBytes", 104857600); // پیش‌فرض 100MB
            _allowedExtensions = _configuration.GetSection("FileUpload:AllowedExtensions").Get<string[]>() 
                ?? new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".jpg", ".jpeg", ".png", ".zip", ".rar", ".txt" };
            _uploadBasePath = _configuration.GetValue<string>("FileUpload:UploadPath", "Uploads") ?? "Uploads";
        }

        public async Task<(bool Success, string Message, DocumentMetadata? Metadata)> UploadFileAsync(
            IFormFile file, 
            int? eventId, 
            int? taskId, 
            string uploadedBy, 
            string description)
        {
            try
            {
                // اعتبارسنجی فایل
                if (file == null || file.Length == 0)
                    return (false, "فایل انتخاب نشده است", null);

                // بررسی حجم فایل
                if (file.Length > _maxFileSizeBytes)
                    return (false, $"حجم فایل نباید بیشتر از {_maxFileSizeBytes / 1024 / 1024} مگابایت باشد", null);

                // بررسی پسوند فایل
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!_allowedExtensions.Contains(fileExtension))
                    return (false, $"پسوند فایل مجاز نیست. فرمت‌های مجاز: {string.Join(", ", _allowedExtensions)}", null);

                // ایجاد مسیر ذخیره‌سازی
                var uploadFolder = Path.Combine(_environment.ContentRootPath, _uploadBasePath);
                
                // ایجاد پوشه براساس سال و ماه
                var yearMonth = DateTime.Now.ToString("yyyy-MM");
                var fullUploadPath = Path.Combine(uploadFolder, yearMonth);
                
                if (!Directory.Exists(fullUploadPath))
                    Directory.CreateDirectory(fullUploadPath);

                // ایجاد نام فایل یکتا
                var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(fullUploadPath, uniqueFileName);

                // ذخیره فایل
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // ثبت اطلاعات در دیتابیس
                var relativePath = Path.Combine(_uploadBasePath, yearMonth, uniqueFileName);
                var metadata = new DocumentMetadata
                {
                    FileName = file.FileName,
                    FileType = fileExtension,
                    FileSize = file.Length,
                    FilePath = relativePath,
                    EventId = eventId,
                    TaskId = taskId,
                    UploadedBy = uploadedBy,
                    Description = description,
                    CreatedBy = uploadedBy,
                    CreatedAt = DateTime.Now
                };

                _context.DocumentMetadatas.Add(metadata);
                await _context.SaveChangesAsync();

                Log.Information($"فایل آپلود شد: {file.FileName} توسط {uploadedBy}");
                return (true, "فایل با موفقیت آپلود شد", metadata);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "خطا در آپلود فایل");
                return (false, "خطای سیستمی در آپلود فایل رخ داد", null);
            }
        }

        public async Task<(bool Success, string Message)> DeleteFileAsync(int documentId, string deletedBy)
        {
            try
            {
                var document = await _context.DocumentMetadatas.FindAsync(documentId);
                if (document == null || document.IsDeleted)
                    return (false, "فایل یافت نشد");

                // حذف فایل فیزیکی
                var fullPath = Path.Combine(_environment.ContentRootPath, document.FilePath);
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }

                // حذف نرم از دیتابیس
                document.IsDeleted = true;
                document.UpdatedBy = deletedBy;
                document.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                Log.Information($"فایل حذف شد: {document.FileName} توسط {deletedBy}");
                return (true, "فایل با موفقیت حذف شد");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در حذف فایل {documentId}");
                return (false, "خطای سیستمی در حذف فایل رخ داد");
            }
        }

        public async Task<(bool Success, string Message, string? FilePath)> GetFilePathAsync(int documentId)
        {
            try
            {
                var document = await _context.DocumentMetadatas.FindAsync(documentId);
                if (document == null || document.IsDeleted)
                    return (false, "فایل یافت نشد", null);

                var fullPath = Path.Combine(_environment.ContentRootPath, document.FilePath);
                if (!File.Exists(fullPath))
                    return (false, "فایل فیزیکی یافت نشد", null);

                return (true, "موفق", fullPath);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در دریافت مسیر فایل {documentId}");
                return (false, "خطای سیستمی رخ داد", null);
            }
        }
    }
}
