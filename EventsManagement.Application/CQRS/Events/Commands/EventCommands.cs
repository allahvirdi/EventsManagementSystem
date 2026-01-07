using MediatR;
using EventsManagement.Shared.DTOs;

namespace EventsManagement.Application.CQRS.Events.Commands
{
    /// <summary>
    /// Command برای ایجاد رویداد جدید
    /// </summary>
    public class CreateEventCommand : IRequest<EventDto>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int EventSourceId { get; set; }
        public int EventSubjectId { get; set; }
        public int UrgencyId { get; set; }
        public int ScopeId { get; set; }
        public string ScopeDetails { get; set; }
        public int ImpactScopeId { get; set; }
        public string ImpactScopeDetails { get; set; }
        public int ImpactRangeId { get; set; }
        public DateTime EventStartDate { get; set; }
        public DateTime? EventEndDate { get; set; }
        public int ActionUnitId { get; set; }
        public string CreatedBy { get; set; }

        public CreateEventCommand(CreateEventDto dto, string createdBy)
        {
            Title = dto.Title;
            Description = dto.Description;
            EventSourceId = dto.EventSourceId;
            EventSubjectId = dto.EventSubjectId;
            UrgencyId = dto.UrgencyId;
            ScopeId = dto.ScopeId;
            ScopeDetails = dto.ScopeDetails;
            ImpactScopeId = dto.ImpactScopeId;
            ImpactScopeDetails = dto.ImpactScopeDetails;
            ImpactRangeId = dto.ImpactRangeId;
            EventStartDate = dto.EventStartDate;
            EventEndDate = dto.EventEndDate;
            ActionUnitId = dto.ActionUnitId;
            CreatedBy = createdBy;
        }
    }

    /// <summary>
    /// Command برای به‌روزرسانی رویداد
    /// </summary>
    public class UpdateEventCommand : IRequest<EventDto>
    {
        public int EventId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int EventSourceId { get; set; }
        public int EventSubjectId { get; set; }
        public int UrgencyId { get; set; }
        public int ScopeId { get; set; }
        public string ScopeDetails { get; set; }
        public int ImpactScopeId { get; set; }
        public string ImpactScopeDetails { get; set; }
        public int ImpactRangeId { get; set; }
        public DateTime EventStartDate { get; set; }
        public DateTime? EventEndDate { get; set; }
        public int ActionUnitId { get; set; }
        public string UpdatedBy { get; set; }
    }

    /// <summary>
    /// Command برای حذف رویداد
    /// </summary>
    public class DeleteEventCommand : IRequest<bool>
    {
        public int EventId { get; set; }
        public string DeletedBy { get; set; }

        public DeleteEventCommand(int eventId, string deletedBy)
        {
            EventId = eventId;
            DeletedBy = deletedBy;
        }
    }

    /// <summary>
    /// Command برای تغییر وضعیت رویداد
    /// </summary>
    public class UpdateEventStatusCommand : IRequest<EventDto>
    {
        public int EventId { get; set; }
        public int StatusId { get; set; }
        public string UpdatedBy { get; set; }

        public UpdateEventStatusCommand(int eventId, int statusId, string updatedBy)
        {
            EventId = eventId;
            StatusId = statusId;
            UpdatedBy = updatedBy;
        }
    }
}
