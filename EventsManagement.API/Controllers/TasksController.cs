using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using EventsManagement.Shared.DTOs;
using EventsManagement.Application.CQRS.Tasks.Commands;
using EventsManagement.Application.CQRS.Tasks.Queries;
using Serilog;

namespace EventsManagement.API.Controllers
{
    /// <summary>
    /// کنترلر مدیریت وظایف
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TasksController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// دریافت لیست تمام وظایف
        /// GET: api/Tasks
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResult<List<TaskDto>>>> GetAllTasks()
        {
            try
            {
                var query = new GetAllTasksQuery();
                var tasks = await _mediator.Send(query);
                return Ok(ApiResult<List<TaskDto>>.Success(tasks, $"{tasks.Count} وظیفه یافت شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "خطا در دریافت لیست وظایف");
                return Ok(ApiResult<List<TaskDto>>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// دریافت جزئیات یک وظیفه
        /// GET: api/Tasks/{id}
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<TaskDto>>> GetTaskById(int id)
        {
            try
            {
                var query = new GetTaskByIdQuery(id);
                var taskDto = await _mediator.Send(query);

                if (taskDto == null)
                    return Ok(ApiResult<TaskDto>.Failure("وظیفه یافت نشد", 404));

                return Ok(ApiResult<TaskDto>.Success(taskDto));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در دریافت وظیفه {id}");
                return Ok(ApiResult<TaskDto>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// دریافت وظایف یک رویداد
        /// GET: api/Tasks/ByEvent/{eventId}
        /// </summary>
        [HttpGet("ByEvent/{eventId}")]
        public async Task<ActionResult<ApiResult<List<TaskDto>>>> GetTasksByEvent(int eventId)
        {
            try
            {
                var query = new GetTasksByEventQuery(eventId);
                var tasks = await _mediator.Send(query);
                return Ok(ApiResult<List<TaskDto>>.Success(tasks, $"{tasks.Count} وظیفه یافت شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در دریافت وظایف رویداد {eventId}");
                return Ok(ApiResult<List<TaskDto>>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// دریافت وظایف واگذار شده به واحد سازمانی
        /// GET: api/Tasks/ByUnit/{unitId}
        /// </summary>
        [HttpGet("ByUnit/{unitId}")]
        public async Task<ActionResult<ApiResult<List<TaskDto>>>> GetTasksByUnit(int unitId)
        {
            try
            {
                var query = new GetTasksByUnitQuery(unitId);
                var tasks = await _mediator.Send(query);
                return Ok(ApiResult<List<TaskDto>>.Success(tasks, $"{tasks.Count} وظیفه یافت شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در دریافت وظایف واحد {unitId}");
                return Ok(ApiResult<List<TaskDto>>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// دریافت وظایف با وضعیت خاص
        /// GET: api/Tasks/ByStatus/{statusId}
        /// </summary>
        [HttpGet("ByStatus/{statusId}")]
        public async Task<ActionResult<ApiResult<List<TaskDto>>>> GetTasksByStatus(int statusId)
        {
            try
            {
                var query = new GetTasksByStatusQuery(statusId);
                var tasks = await _mediator.Send(query);
                return Ok(ApiResult<List<TaskDto>>.Success(tasks, $"{tasks.Count} وظیفه یافت شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در دریافت وظایف با وضعیت {statusId}");
                return Ok(ApiResult<List<TaskDto>>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// جستجو در وظایف
        /// GET: api/Tasks/Search?searchTerm=xxx
        /// </summary>
        [HttpGet("Search")]
        public async Task<ActionResult<ApiResult<List<TaskDto>>>> SearchTasks([FromQuery] string searchTerm)
        {
            try
            {
                var query = new SearchTasksQuery(searchTerm);
                var tasks = await _mediator.Send(query);
                return Ok(ApiResult<List<TaskDto>>.Success(tasks, $"{tasks.Count} وظیفه یافت شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در جستجوی وظایف: {searchTerm}");
                return Ok(ApiResult<List<TaskDto>>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// دریافت جزئیات کامل وظیفه (با پاسخ‌ها)
        /// GET: api/Tasks/{id}/Details
        /// </summary>
        [HttpGet("{id}/Details")]
        public async Task<ActionResult<ApiResult<TaskDto>>> GetTaskDetails(int id)
        {
            try
            {
                var query = new GetTaskDetailsQuery(id);
                var taskDto = await _mediator.Send(query);

                if (taskDto == null)
                    return Ok(ApiResult<TaskDto>.Failure("وظیفه یافت نشد", 404));

                return Ok(ApiResult<TaskDto>.Success(taskDto));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در دریافت جزئیات وظیفه {id}");
                return Ok(ApiResult<TaskDto>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// ایجاد وظیفه جدید
        /// POST: api/Tasks
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Operator")]
        public async Task<ActionResult<ApiResult<TaskDto>>> CreateTask([FromBody] CreateTaskDto model)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var command = new CreateTaskCommand(model, userId);
                var taskDto = await _mediator.Send(command);

                return Ok(ApiResult<TaskDto>.Success(taskDto, "وظیفه با موفقیت ایجاد شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "خطا در ایجاد وظیفه");
                return Ok(ApiResult<TaskDto>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// ویرایش وظیفه
        /// PUT: api/Tasks/{id}
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager,Operator")]
        public async Task<ActionResult<ApiResult<TaskDto>>> UpdateTask(int id, [FromBody] CreateTaskDto model)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var command = new UpdateTaskCommand(id, model, userId);
                var taskDto = await _mediator.Send(command);

                if (taskDto == null)
                    return Ok(ApiResult<TaskDto>.Failure("وظیفه یافت نشد یا حذف شده است", 404));

                return Ok(ApiResult<TaskDto>.Success(taskDto, "وظیفه با موفقیت ویرایش شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در ویرایش وظیفه {id}");
                return Ok(ApiResult<TaskDto>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// حذف وظیفه
        /// DELETE: api/Tasks/{id}
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ApiResult>> DeleteTask(int id)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var command = new DeleteTaskCommand(id, userId);
                var result = await _mediator.Send(command);

                if (!result)
                    return Ok(ApiResult.Failure("وظیفه یافت نشد یا قبلا حذف شده است", 404));

                return Ok(ApiResult.Success("وظیفه با موفقیت حذف شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در حذف وظیفه {id}");
                return Ok(ApiResult.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// به‌روزرسانی درصد پیشرفت وظیفه
        /// PATCH: api/Tasks/{id}/Progress
        /// </summary>
        [HttpPatch("{id}/Progress")]
        [Authorize(Roles = "Admin,Manager,Operator")]
        public async Task<ActionResult<ApiResult<TaskDto>>> UpdateTaskProgress(int id, [FromBody] UpdateProgressDto model)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var command = new UpdateTaskProgressCommand(id, model.ProgressPercentage, model.CalculationMethod, userId);
                var taskDto = await _mediator.Send(command);

                if (taskDto == null)
                    return Ok(ApiResult<TaskDto>.Failure("وظیفه یافت نشد یا حذف شده است", 404));

                return Ok(ApiResult<TaskDto>.Success(taskDto, "درصد پیشرفت وظیفه با موفقیت به‌روزرسانی شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در به‌روزرسانی درصد پیشرفت وظیفه {id}");
                return Ok(ApiResult<TaskDto>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// تغییر وضعیت وظیفه
        /// PATCH: api/Tasks/{id}/Status
        /// </summary>
        [HttpPatch("{id}/Status")]
        [Authorize(Roles = "Admin,Manager,Operator")]
        public async Task<ActionResult<ApiResult<TaskDto>>> UpdateTaskStatus(int id, [FromBody] UpdateTaskStatusDto model)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var command = new UpdateTaskStatusCommand(id, model.StatusId, userId);
                var taskDto = await _mediator.Send(command);

                if (taskDto == null)
                    return Ok(ApiResult<TaskDto>.Failure("وظیفه یافت نشد یا حذف شده است", 404));

                return Ok(ApiResult<TaskDto>.Success(taskDto, "وضعیت وظیفه با موفقیت تغییر یافت"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در تغییر وضعیت وظیفه {id}");
                return Ok(ApiResult<TaskDto>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }
    }

    /// <summary>
    /// مدل به‌روزرسانی درصد پیشرفت
    /// </summary>
    public class UpdateProgressDto
    {
        public int ProgressPercentage { get; set; }
        public string CalculationMethod { get; set; }
    }

    /// <summary>
    /// مدل تغییر وضعیت وظیفه
    /// </summary>
    public class UpdateTaskStatusDto
    {
        public int StatusId { get; set; }
    }
}
