using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using EventsManagement.Infrastructure.Repositories;
using EventsManagement.Shared.DTOs;
using EventsManagement.Application.CQRS.Events.Queries;

namespace EventsManagement.Application.CQRS.Events.Handlers
{
    public class GetAllEventsHandler : IRequestHandler<GetAllEventsQuery, List<EventDto>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IMapper _mapper;

        public GetAllEventsHandler(IEventRepository eventRepository, IMapper mapper)
        {
            _eventRepository = eventRepository;
            _mapper = mapper;
        }

        public async Task<List<EventDto>> Handle(GetAllEventsQuery request, CancellationToken cancellationToken)
        {
            var events = await _eventRepository.AsQueryable()
                .Where(e => !e.IsDeleted)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<EventDto>>(events);
        }
    }

    public class GetEventByIdHandler : IRequestHandler<GetEventByIdQuery, EventDto>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IMapper _mapper;

        public GetEventByIdHandler(IEventRepository eventRepository, IMapper mapper)
        {
            _eventRepository = eventRepository;
            _mapper = mapper;
        }

        public async Task<EventDto> Handle(GetEventByIdQuery request, CancellationToken cancellationToken)
        {
            var eventEntity = await _eventRepository.GetByIdAsync(request.EventId);
            if (eventEntity == null || eventEntity.IsDeleted)
                throw new Exception($"رویداد با شناسه {request.EventId} پیدا نشد");

            return _mapper.Map<EventDto>(eventEntity);
        }
    }

    public class GetEventsByUnitHandler : IRequestHandler<GetEventsByUnitQuery, List<EventDto>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IMapper _mapper;

        public GetEventsByUnitHandler(IEventRepository eventRepository, IMapper mapper)
        {
            _eventRepository = eventRepository;
            _mapper = mapper;
        }

        public async Task<List<EventDto>> Handle(GetEventsByUnitQuery request, CancellationToken cancellationToken)
        {
            var events = await _eventRepository.GetEventsByUnitAsync(request.UnitId);
            return _mapper.Map<List<EventDto>>(events);
        }
    }

    public class GetEventsByStatusHandler : IRequestHandler<GetEventsByStatusQuery, List<EventDto>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IMapper _mapper;

        public GetEventsByStatusHandler(IEventRepository eventRepository, IMapper mapper)
        {
            _eventRepository = eventRepository;
            _mapper = mapper;
        }

        public async Task<List<EventDto>> Handle(GetEventsByStatusQuery request, CancellationToken cancellationToken)
        {
            var events = await _eventRepository.GetEventsByStatusAsync(request.StatusId);
            return _mapper.Map<List<EventDto>>(events);
        }
    }

    public class SearchEventsHandler : IRequestHandler<SearchEventsQuery, List<EventDto>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IMapper _mapper;

        public SearchEventsHandler(IEventRepository eventRepository, IMapper mapper)
        {
            _eventRepository = eventRepository;
            _mapper = mapper;
        }

        public async Task<List<EventDto>> Handle(SearchEventsQuery request, CancellationToken cancellationToken)
        {
            var events = await _eventRepository.SearchEventsAsync(request.Keyword);
            return _mapper.Map<List<EventDto>>(events);
        }
    }

    public class GetEventDetailsHandler : IRequestHandler<GetEventDetailsQuery, EventDto>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IMapper _mapper;

        public GetEventDetailsHandler(IEventRepository eventRepository, IMapper mapper)
        {
            _eventRepository = eventRepository;
            _mapper = mapper;
        }

        public async Task<EventDto> Handle(GetEventDetailsQuery request, CancellationToken cancellationToken)
        {
            var eventEntity = await _eventRepository.GetEventWithDetailsAsync(request.EventId);
            if (eventEntity == null)
                throw new Exception($"رویداد با شناسه {request.EventId} پیدا نشد");

            return _mapper.Map<EventDto>(eventEntity);
        }
    }
}
