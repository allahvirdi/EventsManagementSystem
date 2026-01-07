using MediatR;
using EventsManagement.Shared.DTOs;

namespace EventsManagement.Application.CQRS.Tasks.Queries
{
    /// <summary>
    /// Query برای دریافت تمام وظایف
    /// </summary>
    public class GetAllTasksQuery : IRequest<List<TaskDto>>
    {
    }

    /// <summary>
    /// Query برای دریافت وظیفه با شناسه
    /// </summary>
    public class GetTaskByIdQuery : IRequest<TaskDto>
    {
        public int TaskId { get; set; }

        public GetTaskByIdQuery(int taskId)
        {
            TaskId = taskId;
        }
    }

    /// <summary>
    /// Query برای دریافت وظایف یک رویداد
    /// </summary>
    public class GetTasksByEventQuery : IRequest<List<TaskDto>>
    {
        public int EventId { get; set; }

        public GetTasksByEventQuery(int eventId)
        {
            EventId = eventId;
        }
    }

    /// <summary>
    /// Query برای دریافت وظایف اختصاص‌یافته به واحد
    /// </summary>
    public class GetTasksByUnitQuery : IRequest<List<TaskDto>>
    {
        public int UnitId { get; set; }

        public GetTasksByUnitQuery(int unitId)
        {
            UnitId = unitId;
        }
    }

    /// <summary>
    /// Query برای دریافت وظایف بر اساس وضعیت
    /// </summary>
    public class GetTasksByStatusQuery : IRequest<List<TaskDto>>
    {
        public int StatusId { get; set; }

        public GetTasksByStatusQuery(int statusId)
        {
            StatusId = statusId;
        }
    }

    /// <summary>
    /// Query برای دریافت وظیفه با جزئیات
    /// </summary>
    public class GetTaskDetailsQuery : IRequest<TaskDto>
    {
        public int TaskId { get; set; }

        public GetTaskDetailsQuery(int taskId)
        {
            TaskId = taskId;
        }
    }
}
