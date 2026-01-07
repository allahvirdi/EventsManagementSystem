using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using EventsManagement.Shared.DTOs;
using EventsManagement.Shared.Entities;
using EventsManagement.Infrastructure.Data;
using Serilog;

namespace EventsManagement.API.Controllers
{
    /// <summary>
    /// کنترلر مدیریت نظرات رویدادها
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CommentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CommentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// دریافت نظرات یک رویداد
        /// GET: api/Comments/ByEvent/{eventId}
        /// </summary>
        [HttpGet("ByEvent/{eventId}")]
        public async Task<ActionResult<ApiResult<List<object>>>> GetByEvent(int eventId)
        {
            try
            {
                var comments = await _context.Comments
                    .Where(c => c.EventId == eventId && !c.IsDeleted)
                    .OrderByDescending(c => c.CreatedAt)
                    .Select(c => new
                    {
                        c.Id,
                        c.Content,
                        c.CommentedBy,
                        c.CreatedAt,
                        c.ParentCommentId
                    })
                    .ToListAsync();

                return Ok(ApiResult<List<object>>.Success(comments.Cast<object>().ToList(), $"{comments.Count} نظر یافت شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در دریافت نظرات رویداد {eventId}");
                return Ok(ApiResult<List<object>>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// دریافت یک نظر
        /// GET: api/Comments/{id}
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<object>>> GetById(int id)
        {
            try
            {
                var comment = await _context.Comments
                    .Where(c => c.Id == id && !c.IsDeleted)
                    .Select(c => new
                    {
                        c.Id,
                        c.EventId,
                        c.Content,
                        c.CommentedBy,
                        c.CreatedAt,
                        c.ParentCommentId
                    })
                    .FirstOrDefaultAsync();

                if (comment == null)
                    return Ok(ApiResult<object>.Failure("نظر یافت نشد", 404));

                return Ok(ApiResult<object>.Success(comment));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در دریافت نظر {id}");
                return Ok(ApiResult<object>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// ایجاد نظر جدید
        /// POST: api/Comments
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Operator,Viewer")]
        public async Task<ActionResult<ApiResult<object>>> Create([FromBody] CreateCommentDto model)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var userName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

                if (userId == null)
                    return Ok(ApiResult<object>.Failure("کاربر احراز هویت نشده است", 401));

                // بررسی وجود رویداد
                var eventExists = await _context.Events.AnyAsync(e => e.Id == model.EventId && !e.IsDeleted);
                if (!eventExists)
                    return Ok(ApiResult<object>.Failure("رویداد یافت نشد", 404));

                // اگر نظر پاسخ به نظر دیگری است، بررسی وجود نظر والد
                if (model.ParentCommentId.HasValue)
                {
                    var parentExists = await _context.Comments.AnyAsync(c => c.Id == model.ParentCommentId && !c.IsDeleted);
                    if (!parentExists)
                        return Ok(ApiResult<object>.Failure("نظر والد یافت نشد", 404));
                }

                var comment = new Comment
                {
                    EventId = model.EventId,
                    Content = model.Content,
                    ParentCommentId = model.ParentCommentId,
                    CommentedBy = userName ?? userId,
                    CreatedBy = userId,
                    CreatedAt = DateTime.Now
                };

                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();

                var result = new
                {
                    comment.Id,
                    comment.EventId,
                    comment.Content,
                    comment.CommentedBy,
                    comment.CreatedAt,
                    comment.ParentCommentId
                };

                Log.Information($"نظر جدید برای رویداد {model.EventId} ثبت شد");
                return Ok(ApiResult<object>.Success(result, "نظر با موفقیت ثبت شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "خطا در ثبت نظر");
                return Ok(ApiResult<object>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// ویرایش نظر
        /// PUT: api/Comments/{id}
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<object>>> Update(int id, [FromBody] UpdateCommentDto model)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                var comment = await _context.Comments.FindAsync(id);
                if (comment == null || comment.IsDeleted)
                    return Ok(ApiResult<object>.Failure("نظر یافت نشد", 404));

                // فقط صاحب نظر یا Admin می‌توانند ویرایش کنند
                var isAdmin = User.IsInRole("Admin");
                if (comment.CreatedBy != userId && !isAdmin)
                    return Ok(ApiResult<object>.Failure("شما مجاز به ویرایش این نظر نیستید", 403));

                comment.Content = model.Content;
                comment.UpdatedBy = userId;
                comment.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                var result = new
                {
                    comment.Id,
                    comment.EventId,
                    comment.Content,
                    comment.CommentedBy,
                    comment.CreatedAt,
                    comment.ParentCommentId
                };

                Log.Information($"نظر {id} ویرایش شد");
                return Ok(ApiResult<object>.Success(result, "نظر با موفقیت ویرایش شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در ویرایش نظر {id}");
                return Ok(ApiResult<object>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// حذف نظر
        /// DELETE: api/Comments/{id}
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult>> Delete(int id)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                var comment = await _context.Comments.FindAsync(id);
                if (comment == null || comment.IsDeleted)
                    return Ok(ApiResult.Failure("نظر یافت نشد", 404));

                // فقط صاحب نظر یا Admin می‌توانند حذف کنند
                var isAdmin = User.IsInRole("Admin");
                if (comment.CreatedBy != userId && !isAdmin)
                    return Ok(ApiResult.Failure("شما مجاز به حذف این نظر نیستید", 403));

                comment.IsDeleted = true;
                comment.UpdatedBy = userId;
                comment.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                Log.Information($"نظر {id} حذف شد");
                return Ok(ApiResult.Success("نظر با موفقیت حذف شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در حذف نظر {id}");
                return Ok(ApiResult.Failure("خطای سیستمی رخ داده است", 500));
            }
        }
    }

    /// <summary>
    /// DTO برای ایجاد نظر
    /// </summary>
    public class CreateCommentDto
    {
        public int EventId { get; set; }
        public string Content { get; set; }
        public int? ParentCommentId { get; set; }
    }

    /// <summary>
    /// DTO برای ویرایش نظر
    /// </summary>
    public class UpdateCommentDto
    {
        public string Content { get; set; }
    }
}
