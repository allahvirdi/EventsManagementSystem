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
    /// کنترلر مدیریت پاسخ‌های وظایف
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TaskRepliesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TaskRepliesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// دریافت لیست پاسخ‌های یک وظیفه
        /// GET: api/TaskReplies/ByTask/{taskId}
        /// </summary>
        [HttpGet("ByTask/{taskId}")]
        public async Task<ActionResult<ApiResult<List<TaskReplyDto>>>> GetRepliesByTask(int taskId)
        {
            try
            {
                var replies = await _context.TaskReplies
                    .Where(r => r.TaskId == taskId && !r.IsDeleted)
                    .OrderBy(r => r.ActionDateTime)
                    .Select(r => new TaskReplyDto
                    {
                        Id = r.Id,
                        TaskId = r.TaskId,
                        Content = r.Content,
                        ActionDateTime = r.ActionDateTime,
                        RespondedByUserId = r.RespondedByUserId,
                        RegisteredDate = r.RegisteredDate,
                        SupervisorUnitId = r.SupervisorUnitId
                    })
                    .ToListAsync();

                return Ok(ApiResult<List<TaskReplyDto>>.Success(replies, $"{replies.Count} پاسخ یافت شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در دریافت پاسخ‌های وظیفه {taskId}");
                return Ok(ApiResult<List<TaskReplyDto>>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// دریافت یک پاسخ
        /// GET: api/TaskReplies/{id}
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<TaskReplyDto>>> GetById(int id)
        {
            try
            {
                var reply = await _context.TaskReplies
                    .Where(r => r.Id == id && !r.IsDeleted)
                    .Select(r => new TaskReplyDto
                    {
                        Id = r.Id,
                        TaskId = r.TaskId,
                        Content = r.Content,
                        ActionDateTime = r.ActionDateTime,
                        RespondedByUserId = r.RespondedByUserId,
                        RegisteredDate = r.RegisteredDate,
                        SupervisorUnitId = r.SupervisorUnitId
                    })
                    .FirstOrDefaultAsync();

                if (reply == null)
                    return Ok(ApiResult<TaskReplyDto>.Failure("پاسخ یافت نشد", 404));

                return Ok(ApiResult<TaskReplyDto>.Success(reply));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در دریافت پاسخ {id}");
                return Ok(ApiResult<TaskReplyDto>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// ایجاد پاسخ جدید
        /// POST: api/TaskReplies
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Operator")]
        public async Task<ActionResult<ApiResult<TaskReplyDto>>> Create([FromBody] CreateTaskReplyDto model)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                // بررسی وجود وظیفه
                var taskExists = await _context.EventTasks.AnyAsync(t => t.Id == model.TaskId && !t.IsDeleted);
                if (!taskExists)
                    return Ok(ApiResult<TaskReplyDto>.Failure("وظیفه یافت نشد", 404));

                var reply = new TaskReply
                {
                    TaskId = model.TaskId,
                    Content = model.Content,
                    ActionDateTime = model.ActionDateTime,
                    RespondedByUserId = userId,
                    RegisteredDate = DateTime.Now,
                    SupervisorUnitId = model.SupervisorUnitId,
                    CreatedBy = userId,
                    CreatedAt = DateTime.Now
                };

                _context.TaskReplies.Add(reply);
                await _context.SaveChangesAsync();

                var dto = new TaskReplyDto
                {
                    Id = reply.Id,
                    TaskId = reply.TaskId,
                    Content = reply.Content,
                    ActionDateTime = reply.ActionDateTime,
                    RespondedByUserId = reply.RespondedByUserId,
                    RegisteredDate = reply.RegisteredDate,
                    SupervisorUnitId = reply.SupervisorUnitId
                };

                Log.Information($"پاسخ جدید برای وظیفه {model.TaskId} ثبت شد");
                return Ok(ApiResult<TaskReplyDto>.Success(dto, "پاسخ با موفقیت ثبت شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "خطا در ثبت پاسخ");
                return Ok(ApiResult<TaskReplyDto>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// ویرایش پاسخ
        /// PUT: api/TaskReplies/{id}
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager,Operator")]
        public async Task<ActionResult<ApiResult<TaskReplyDto>>> Update(int id, [FromBody] CreateTaskReplyDto model)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                var reply = await _context.TaskReplies.FindAsync(id);
                if (reply == null || reply.IsDeleted)
                    return Ok(ApiResult<TaskReplyDto>.Failure("پاسخ یافت نشد", 404));

                reply.Content = model.Content;
                reply.ActionDateTime = model.ActionDateTime;
                reply.SupervisorUnitId = model.SupervisorUnitId;
                reply.UpdatedBy = userId;
                reply.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                var dto = new TaskReplyDto
                {
                    Id = reply.Id,
                    TaskId = reply.TaskId,
                    Content = reply.Content,
                    ActionDateTime = reply.ActionDateTime,
                    RespondedByUserId = reply.RespondedByUserId,
                    RegisteredDate = reply.RegisteredDate,
                    SupervisorUnitId = reply.SupervisorUnitId
                };

                Log.Information($"پاسخ {id} ویرایش شد");
                return Ok(ApiResult<TaskReplyDto>.Success(dto, "پاسخ با موفقیت ویرایش شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در ویرایش پاسخ {id}");
                return Ok(ApiResult<TaskReplyDto>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// حذف پاسخ
        /// DELETE: api/TaskReplies/{id}
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ApiResult>> Delete(int id)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                var reply = await _context.TaskReplies.FindAsync(id);
                if (reply == null || reply.IsDeleted)
                    return Ok(ApiResult.Failure("پاسخ یافت نشد", 404));

                reply.IsDeleted = true;
                reply.UpdatedBy = userId;
                reply.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                Log.Information($"پاسخ {id} حذف شد");
                return Ok(ApiResult.Success("پاسخ با موفقیت حذف شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در حذف پاسخ {id}");
                return Ok(ApiResult.Failure("خطای سیستمی رخ داده است", 500));
            }
        }
    }
}
