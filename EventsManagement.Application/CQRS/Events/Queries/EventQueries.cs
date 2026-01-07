using MediatR;
using EventsManagement.Shared.DTOs;

namespace EventsManagement.Application.CQRS.Events.Queries
{
    /// <summary>
    /// Query برای دریافت تمام رویدادها
    /// </summary>
    public class GetAllEventsQuery : IRequest<List<EventDto>>
    {
    }

    /// <summary>
    /// Query برای دریافت رویداد با شناسه
    /// </summary>
    public class GetEventByIdQuery : IRequest<EventDto>
    {
        public int EventId { get; set; }

        public GetEventByIdQuery(int eventId)
        {
            EventId = eventId;
        }
    }

    /// <summary>
    /// Query برای دریافت رویدادهای یک واحد
    /// </summary>
    public class GetEventsByUnitQuery : IRequest<List<EventDto>>
    {
        public int UnitId { get; set; }

        public GetEventsByUnitQuery(int unitId)
        {
            UnitId = unitId;
        }
    }

    /// <summary>
    /// Query برای دریافت رویدادهای بر اساس وضعیت
    /// </summary>
    public class GetEventsByStatusQuery : IRequest<List<EventDto>>
    {
        public int StatusId { get; set; }

        public GetEventsByStatusQuery(int statusId)
        {
            StatusId = statusId;
        }
    }

    /// <summary>
    /// Query برای جستجو رویدادها
    /// </summary>
    public class SearchEventsQuery : IRequest<List<EventDto>>
    {
        public string Keyword { get; set; }

        public SearchEventsQuery(string keyword)
        {
            Keyword = keyword;
        }
    }

    /// <summary>
    /// Query برای دریافت رویداد با تمام جزئیات
    /// </summary>
    public class GetEventDetailsQuery : IRequest<EventDto>
    {
        public int EventId { get; set; }

        public GetEventDetailsQuery(int eventId)
        {
            EventId = eventId;
        }
    }
}
