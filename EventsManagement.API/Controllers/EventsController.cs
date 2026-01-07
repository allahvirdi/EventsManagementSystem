using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using EventsManagement.Shared.DTOs;
using EventsManagement.Application.CQRS.Events.Commands;
using EventsManagement.Application.CQRS.Events.Queries;
using Serilog;

namespace EventsManagement.API.Controllers
{
    /// <summary>
    /// کنترلر مدیریت رویدادها
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EventsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// دریافت لیست تمام رویدادها
        /// GET: api/Events
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResult<List<EventDto>>>> GetAllEvents()
        {
            try
            {
                var query = new GetAllEventsQuery();
                var events = await _mediator.Send(query);
                return Ok(ApiResult<List<EventDto>>.Success(events, $"{events.Count} رویداد یافت شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "خطا در دریافت لیست رویدادها");
                return Ok(ApiResult<List<EventDto>>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// دریافت جزئیات یک رویداد
        /// GET: api/Events/{id}
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<EventDto>>> GetEventById(int id)
        {
            try
            {
                var query = new GetEventByIdQuery(id);
                var eventDto = await _mediator.Send(query);

                if (eventDto == null)
                    return Ok(ApiResult<EventDto>.Failure("رویداد یافت نشد", 404));

                return Ok(ApiResult<EventDto>.Success(eventDto));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در دریافت رویداد {id}");
                return Ok(ApiResult<EventDto>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// دریافت رویدادهای یک واحد سازمانی
        /// GET: api/Events/ByUnit/{unitId}
        /// </summary>
        [HttpGet("ByUnit/{unitId}")]
        public async Task<ActionResult<ApiResult<List<EventDto>>>> GetEventsByUnit(int unitId)
        {
            try
            {
                var query = new GetEventsByUnitQuery(unitId);
                var events = await _mediator.Send(query);
                return Ok(ApiResult<List<EventDto>>.Success(events, $"{events.Count} رویداد یافت شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در دریافت رویدادهای واحد {unitId}");
                return Ok(ApiResult<List<EventDto>>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// دریافت رویدادهای با وضعیت خاص
        /// GET: api/Events/ByStatus/{statusId}
        /// </summary>
        [HttpGet("ByStatus/{statusId}")]
        public async Task<ActionResult<ApiResult<List<EventDto>>>> GetEventsByStatus(int statusId)
        {
            try
            {
                var query = new GetEventsByStatusQuery(statusId);
                var events = await _mediator.Send(query);
                return Ok(ApiResult<List<EventDto>>.Success(events, $"{events.Count} رویداد یافت شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در دریافت رویدادهای با وضعیت {statusId}");
                return Ok(ApiResult<List<EventDto>>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// جستجو در رویدادها
        /// GET: api/Events/Search?searchTerm=xxx
        /// </summary>
        [HttpGet("Search")]
        public async Task<ActionResult<ApiResult<List<EventDto>>>> SearchEvents([FromQuery] string searchTerm)
        {
            try
            {
                var query = new SearchEventsQuery(searchTerm);
                var events = await _mediator.Send(query);
                return Ok(ApiResult<List<EventDto>>.Success(events, $"{events.Count} رویداد یافت شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در جستجوی رویدادها: {searchTerm}");
                return Ok(ApiResult<List<EventDto>>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// دریافت جزئیات کامل یک رویداد (با وظایف و پاسخ‌ها)
        /// GET: api/Events/{id}/Details
        /// </summary>
        [HttpGet("{id}/Details")]
        public async Task<ActionResult<ApiResult<EventDto>>> GetEventDetails(int id)
        {
            try
            {
                var query = new GetEventDetailsQuery(id);
                var eventDto = await _mediator.Send(query);

                if (eventDto == null)
                    return Ok(ApiResult<EventDto>.Failure("رویداد یافت نشد", 404));

                return Ok(ApiResult<EventDto>.Success(eventDto));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در دریافت جزئیات رویداد {id}");
                return Ok(ApiResult<EventDto>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// ایجاد رویداد جدید
        /// POST: api/Events
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Operator")]
        public async Task<ActionResult<ApiResult<EventDto>>> CreateEvent([FromBody] CreateEventDto model)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var command = new CreateEventCommand(model, userId);
                var eventDto = await _mediator.Send(command);

                return Ok(ApiResult<EventDto>.Success(eventDto, "رویداد با موفقیت ایجاد شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "خطا در ایجاد رویداد");
                return Ok(ApiResult<EventDto>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// ویرایش رویداد
        /// PUT: api/Events/{id}
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager,Operator")]
        public async Task<ActionResult<ApiResult<EventDto>>> UpdateEvent(int id, [FromBody] CreateEventDto model)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var command = new UpdateEventCommand(id, model, userId);
                var eventDto = await _mediator.Send(command);

                if (eventDto == null)
                    return Ok(ApiResult<EventDto>.Failure("رویداد یافت نشد یا حذف شده است", 404));

                return Ok(ApiResult<EventDto>.Success(eventDto, "رویداد با موفقیت ویرایش شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در ویرایش رویداد {id}");
                return Ok(ApiResult<EventDto>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// حذف رویداد
        /// DELETE: api/Events/{id}
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ApiResult>> DeleteEvent(int id)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var command = new DeleteEventCommand(id, userId);
                var result = await _mediator.Send(command);

                if (!result)
                    return Ok(ApiResult.Failure("رویداد یافت نشد یا قبلا حذف شده است", 404));

                return Ok(ApiResult.Success("رویداد با موفقیت حذف شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در حذف رویداد {id}");
                return Ok(ApiResult.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// تغییر وضعیت رویداد
        /// PATCH: api/Events/{id}/Status
        /// </summary>
        [HttpPatch("{id}/Status")]
        [Authorize(Roles = "Admin,Manager,Operator")]
        public async Task<ActionResult<ApiResult<EventDto>>> UpdateEventStatus(int id, [FromBody] UpdateEventStatusDto model)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var command = new UpdateEventStatusCommand(id, model.StatusId, userId);
                var eventDto = await _mediator.Send(command);

                if (eventDto == null)
                    return Ok(ApiResult<EventDto>.Failure("رویداد یافت نشد یا حذف شده است", 404));

                return Ok(ApiResult<EventDto>.Success(eventDto, "وضعیت رویداد با موفقیت تغییر یافت"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در تغییر وضعیت رویداد {id}");
                return Ok(ApiResult<EventDto>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }
    }

    /// <summary>
    /// مدل تغییر وضعیت رویداد
    /// </summary>
    public class UpdateEventStatusDto
    {
        public int StatusId { get; set; }
    }
}
