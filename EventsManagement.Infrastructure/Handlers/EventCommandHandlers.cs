using AutoMapper;
using MediatR;
using EventsManagement.Infrastructure.Repositories;
using EventsManagement.Shared.DTOs;
using EventsManagement.Shared.Entities;
using EventsManagement.Application.CQRS.Events.Commands;
using Serilog;

namespace EventsManagement.Application.CQRS.Events.Handlers
{
    public class CreateEventHandler : IRequestHandler<CreateEventCommand, EventDto>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IMapper _mapper;

        public CreateEventHandler(IEventRepository eventRepository, IMapper mapper)
        {
            _eventRepository = eventRepository;
            _mapper = mapper;
        }

        public async Task<EventDto> Handle(CreateEventCommand request, CancellationToken cancellationToken)
        {
            // نقشه‌برداری DTO به Entity
            var eventEntity = new Event
            {
                Title = request.Title,
                Description = request.Description,
                EventSourceId = request.EventSourceId,
                EventSubjectId = request.EventSubjectId,
                UrgencyId = request.UrgencyId,
                ScopeId = request.ScopeId,
                ScopeDetails = request.ScopeDetails,
                ImpactScopeId = request.ImpactScopeId,
                ImpactScopeDetails = request.ImpactScopeDetails,
                ImpactRangeId = request.ImpactRangeId,
                EventStartDate = request.EventStartDate,
                EventEndDate = request.EventEndDate,
                ActionUnitId = request.ActionUnitId,
                RegisteredBy = request.CreatedBy,
                StatusId = 1, // وضعیت پیش‌فرض: ثبت‌شده
                CreatedBy = request.CreatedBy
            };

            // ذخیره‌سازی
            var createdEvent = await _eventRepository.AddAsync(eventEntity);

            // لاگ‌ثبت
            Log.Information($"رویداد جدید ایجاد شد: {createdEvent.Id} توسط {request.CreatedBy}");

            // نقشه‌برداری به DTO
            return _mapper.Map<EventDto>(createdEvent);
        }
    }

    public class UpdateEventHandler : IRequestHandler<UpdateEventCommand, EventDto>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IMapper _mapper;

        public UpdateEventHandler(IEventRepository eventRepository, IMapper mapper)
        {
            _eventRepository = eventRepository;
            _mapper = mapper;
        }

        public async Task<EventDto> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(request.EventId);
            if (eventEntity == null || eventEntity.IsDeleted)
                throw new Exception($"رویداد با شناسه {request.EventId} پیدا نشد");

            // به‌روزرسانی فیلدها
            eventEntity.Title = request.Title;
            eventEntity.Description = request.Description;
            eventEntity.EventSourceId = request.EventSourceId;
            eventEntity.EventSubjectId = request.EventSubjectId;
            eventEntity.UrgencyId = request.UrgencyId;
            eventEntity.ScopeId = request.ScopeId;
            eventEntity.ScopeDetails = request.ScopeDetails;
            eventEntity.ImpactScopeId = request.ImpactScopeId;
            eventEntity.ImpactScopeDetails = request.ImpactScopeDetails;
            eventEntity.ImpactRangeId = request.ImpactRangeId;
            eventEntity.EventStartDate = request.EventStartDate;
            eventEntity.EventEndDate = request.EventEndDate;
            eventEntity.ActionUnitId = request.ActionUnitId;
            eventEntity.UpdatedBy = request.UpdatedBy;

            var updatedEvent = await _eventRepository.UpdateAsync(eventEntity);

            Log.Information($"رویداد {request.EventId} به‌روزرسانی شد توسط {request.UpdatedBy}");

            return _mapper.Map<EventDto>(updatedEvent);
        }
    }

    public class DeleteEventHandler : IRequestHandler<DeleteEventCommand, bool>
    {
        private readonly IEventRepository _eventRepository;

        public DeleteEventHandler(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<bool> Handle(DeleteEventCommand request, CancellationToken cancellationToken)
        {
            var result = await _eventRepository.DeleteAsync(request.EventId);

            if (result)
                Log.Information($"رویداد {request.EventId} حذف شد توسط {request.DeletedBy}");

            return result;
        }
    }

    public class UpdateEventStatusHandler : IRequestHandler<UpdateEventStatusCommand, EventDto>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IMapper _mapper;

        public UpdateEventStatusHandler(IEventRepository eventRepository, IMapper mapper)
        {
            _eventRepository = eventRepository;
            _mapper = mapper;
        }

        public async Task<EventDto> Handle(UpdateEventStatusCommand request, CancellationToken cancellationToken)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(request.EventId);
            if (eventEntity == null || eventEntity.IsDeleted)
                throw new Exception($"رویداد با شناسه {request.EventId} پیدا نشد");

            eventEntity.StatusId = request.StatusId;
            eventEntity.UpdatedBy = request.UpdatedBy;

            var updatedEvent = await _eventRepository.UpdateAsync(eventEntity);

            Log.Information($"وضعیت رویداد {request.EventId} به {request.StatusId} تغییر یافت توسط {request.UpdatedBy}");

            return _mapper.Map<EventDto>(updatedEvent);
        }
    }
}
