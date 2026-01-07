using AutoMapper;
using MediatR;
using EventsManagement.Infrastructure.Repositories;
using EventsManagement.Shared.DTOs;
using EventsManagement.Shared.Entities;
using EventsManagement.Application.CQRS.Tasks.Commands;
using Serilog;

namespace EventsManagement.Infrastructure.Handlers
{
    /// <summary>
    /// Handler برای ایجاد وظیفه جدید
    /// </summary>
    public class CreateTaskHandler : IRequestHandler<CreateTaskCommand, TaskDto>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IMapper _mapper;

        public CreateTaskHandler(ITaskRepository taskRepository, IMapper mapper)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
        }

        public async Task<TaskDto> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
        {
            // ایجاد Entity از DTO
            var task = new EventTask
            {
                EventId = request.EventId,
                Title = request.Title,
                Description = request.Description,
                AssignedToUnitId = request.AssignedToUnitId,
                AssignedToUserId = request.AssignedToUserId,
                SupervisorUserId = request.SupervisorUserId,
                CooperatingUnitsIds = request.CooperatingUnitsIds,
                ActionTypeId = request.ActionTypeId,
                ProgressPercentage = 0,
                DueDate = request.DueDate,
                StatusId = 1, // وضعیت اولیه: جدید
                CreatedBy = request.CreatedBy,
                CreatedAt = DateTime.Now
            };

            await _taskRepository.AddAsync(task);
            await _taskRepository.SaveChangesAsync();

            Log.Information($"وظیفه جدید ایجاد شد: {task.Title} (ID: {task.Id})");

            return _mapper.Map<TaskDto>(task);
        }
    }

    /// <summary>
    /// Handler برای به‌روزرسانی وظیفه
    /// </summary>
    public class UpdateTaskHandler : IRequestHandler<UpdateTaskCommand, TaskDto>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IMapper _mapper;

        public UpdateTaskHandler(ITaskRepository taskRepository, IMapper mapper)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
        }

        public async Task<TaskDto> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
        {
            var task = await _taskRepository.GetByIdAsync(request.TaskId);

            if (task == null || task.IsDeleted)
                return null;

            task.Title = request.Title;
            task.Description = request.Description;
            task.AssignedToUnitId = request.AssignedToUnitId;
            task.AssignedToUserId = request.AssignedToUserId;
            task.SupervisorUserId = request.SupervisorUserId;
            task.CooperatingUnitsIds = request.CooperatingUnitsIds;
            task.ActionTypeId = request.ActionTypeId;
            task.DueDate = request.DueDate;
            task.UpdatedBy = request.UpdatedBy;
            task.UpdatedAt = DateTime.Now;

            await _taskRepository.UpdateAsync(task);
            await _taskRepository.SaveChangesAsync();

            Log.Information($"وظیفه ویرایش شد: {task.Title} (ID: {task.Id})");

            return _mapper.Map<TaskDto>(task);
        }
    }

    /// <summary>
    /// Handler برای حذف وظیفه
    /// </summary>
    public class DeleteTaskHandler : IRequestHandler<DeleteTaskCommand, bool>
    {
        private readonly ITaskRepository _taskRepository;

        public DeleteTaskHandler(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<bool> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
        {
            var task = await _taskRepository.GetByIdAsync(request.TaskId);

            if (task == null || task.IsDeleted)
                return false;

            await _taskRepository.DeleteAsync(request.TaskId);
            await _taskRepository.SaveChangesAsync();

            Log.Information($"وظیفه حذف شد: ID {request.TaskId}");

            return true;
        }
    }

    /// <summary>
    /// Handler برای به‌روزرسانی درصد پیشرفت وظیفه
    /// </summary>
    public class UpdateTaskProgressHandler : IRequestHandler<UpdateTaskProgressCommand, TaskDto>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IMapper _mapper;

        public UpdateTaskProgressHandler(ITaskRepository taskRepository, IMapper mapper)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
        }

        public async Task<TaskDto> Handle(UpdateTaskProgressCommand request, CancellationToken cancellationToken)
        {
            var task = await _taskRepository.GetByIdAsync(request.TaskId);

            if (task == null || task.IsDeleted)
                return null;

            task.ProgressPercentage = request.ProgressPercentage;
            task.ProgressCalculationMethod = request.CalculationMethod;
            task.UpdatedBy = request.UpdatedBy;
            task.UpdatedAt = DateTime.Now;

            // اگر پیشرفت 100% شد، وضعیت را به "تکمیل شده" تغییر بده
            if (request.ProgressPercentage >= 100)
            {
                task.StatusId = 4; // فرض: 4 = تکمیل شده
            }

            await _taskRepository.UpdateAsync(task);
            await _taskRepository.SaveChangesAsync();

            Log.Information($"درصد پیشرفت وظیفه به‌روز شد: {task.Title} - {request.ProgressPercentage}%");

            return _mapper.Map<TaskDto>(task);
        }
    }

    /// <summary>
    /// Handler برای تغییر وضعیت وظیفه
    /// </summary>
    public class UpdateTaskStatusHandler : IRequestHandler<UpdateTaskStatusCommand, TaskDto>
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IMapper _mapper;

        public UpdateTaskStatusHandler(ITaskRepository taskRepository, IMapper mapper)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
        }

        public async Task<TaskDto> Handle(UpdateTaskStatusCommand request, CancellationToken cancellationToken)
        {
            var task = await _taskRepository.GetByIdAsync(request.TaskId);

            if (task == null || task.IsDeleted)
                return null;

            task.StatusId = request.StatusId;
            task.UpdatedBy = request.UpdatedBy;
            task.UpdatedAt = DateTime.Now;

            await _taskRepository.UpdateAsync(task);
            await _taskRepository.SaveChangesAsync();

            Log.Information($"وضعیت وظیفه تغییر یافت: {task.Title} - StatusId: {request.StatusId}");

            return _mapper.Map<TaskDto>(task);
        }
    }
}
