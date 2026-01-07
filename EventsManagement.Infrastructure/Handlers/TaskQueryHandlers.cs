using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using EventsManagement.Infrastructure.Repositories;
using EventsManagement.Shared.DTOs;
using EventsManagement.Application.CQRS.Tasks.Queries;

namespace EventsManagement.Infrastructure.Handlers
{
    /// <summary>
    /// Handler برای دریافت تمام وظایف
    /// </summary>
    public class GetAllTasksHandler : IRequestHandler<GetAllTasksQuery, List<TaskDto>>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IMapper _mapper;

        public GetAllTasksHandler(ITaskRepository taskRepository, IMapper mapper)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
        }

        public async Task<List<TaskDto>> Handle(GetAllTasksQuery request, CancellationToken cancellationToken)
        {
            var tasks = await _taskRepository.AsQueryable()
                .Where(t => !t.IsDeleted)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<TaskDto>>(tasks);
        }
    }

    /// <summary>
    /// Handler برای دریافت وظیفه با شناسه
    /// </summary>
    public class GetTaskByIdHandler : IRequestHandler<GetTaskByIdQuery, TaskDto>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IMapper _mapper;

        public GetTaskByIdHandler(ITaskRepository taskRepository, IMapper mapper)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
        }

        public async Task<TaskDto> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
        {
            var task = await _taskRepository.GetByIdAsync(request.TaskId);

            if (task == null || task.IsDeleted)
                return null;

            return _mapper.Map<TaskDto>(task);
        }
    }

    /// <summary>
    /// Handler برای دریافت وظایف یک رویداد
    /// </summary>
    public class GetTasksByEventHandler : IRequestHandler<GetTasksByEventQuery, List<TaskDto>>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IMapper _mapper;

        public GetTasksByEventHandler(ITaskRepository taskRepository, IMapper mapper)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
        }

        public async Task<List<TaskDto>> Handle(GetTasksByEventQuery request, CancellationToken cancellationToken)
        {
            var tasks = await _taskRepository.GetTasksByEventAsync(request.EventId);
            return _mapper.Map<List<TaskDto>>(tasks);
        }
    }

    /// <summary>
    /// Handler برای دریافت وظایف واحد سازمانی
    /// </summary>
    public class GetTasksByUnitHandler : IRequestHandler<GetTasksByUnitQuery, List<TaskDto>>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IMapper _mapper;

        public GetTasksByUnitHandler(ITaskRepository taskRepository, IMapper mapper)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
        }

        public async Task<List<TaskDto>> Handle(GetTasksByUnitQuery request, CancellationToken cancellationToken)
        {
            var tasks = await _taskRepository.GetTasksByUnitAsync(request.UnitId);
            return _mapper.Map<List<TaskDto>>(tasks);
        }
    }

    /// <summary>
    /// Handler برای دریافت وظایف با وضعیت خاص
    /// </summary>
    public class GetTasksByStatusHandler : IRequestHandler<GetTasksByStatusQuery, List<TaskDto>>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IMapper _mapper;

        public GetTasksByStatusHandler(ITaskRepository taskRepository, IMapper mapper)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
        }

        public async Task<List<TaskDto>> Handle(GetTasksByStatusQuery request, CancellationToken cancellationToken)
        {
            var tasks = await _taskRepository.GetTasksByStatusAsync(request.StatusId);
            return _mapper.Map<List<TaskDto>>(tasks);
        }
    }

    /// <summary>
    /// Handler برای جستجو در وظایف
    /// </summary>
    public class SearchTasksHandler : IRequestHandler<SearchTasksQuery, List<TaskDto>>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IMapper _mapper;

        public SearchTasksHandler(ITaskRepository taskRepository, IMapper mapper)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
        }

        public async Task<List<TaskDto>> Handle(SearchTasksQuery request, CancellationToken cancellationToken)
        {
            var query = _taskRepository.AsQueryable()
                .Where(t => !t.IsDeleted);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(t => t.Title.Contains(request.SearchTerm) || t.Description.Contains(request.SearchTerm));
            }

            var tasks = await query.ToListAsync(cancellationToken);
            return _mapper.Map<List<TaskDto>>(tasks);
        }
    }

    /// <summary>
    /// Handler برای دریافت جزئیات کامل وظیفه
    /// </summary>
    public class GetTaskDetailsHandler : IRequestHandler<GetTaskDetailsQuery, TaskDto>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IMapper _mapper;

        public GetTaskDetailsHandler(ITaskRepository taskRepository, IMapper mapper)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
        }

        public async Task<TaskDto> Handle(GetTaskDetailsQuery request, CancellationToken cancellationToken)
        {
            var task = await _taskRepository.GetTaskWithDetailsAsync(request.TaskId);

            if (task == null || task.IsDeleted)
                return null;

            return _mapper.Map<TaskDto>(task);
        }
    }
}
