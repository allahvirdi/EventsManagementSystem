using MediatR;
using EventsManagement.Shared.DTOs;

namespace EventsManagement.Application.CQRS.Tasks.Commands
{
    /// <summary>
    /// Command برای ایجاد وظیفه جدید
    /// </summary>
    public class CreateTaskCommand : IRequest<TaskDto>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int EventId { get; set; }
        public int AssignedToUnitId { get; set; }
        public string AssignedToUserId { get; set; }
        public string SupervisorUserId { get; set; }
        public string CooperatingUnitsIds { get; set; }
        public int ActionTypeId { get; set; }
        public DateTime? DueDate { get; set; }
        public string CreatedBy { get; set; }

        public CreateTaskCommand(CreateTaskDto dto, string createdBy)
        {
            Title = dto.Title;
            Description = dto.Description;
            EventId = dto.EventId;
            AssignedToUnitId = dto.AssignedToUnitId;
            AssignedToUserId = dto.AssignedToUserId;
            SupervisorUserId = dto.SupervisorUserId;
            CooperatingUnitsIds = dto.CooperatingUnitsIds;
            ActionTypeId = dto.ActionTypeId;
            DueDate = dto.DueDate;
            CreatedBy = createdBy;
        }
    }

    /// <summary>
    /// Command برای به‌روزرسانی وظیفه
    /// </summary>
    public class UpdateTaskCommand : IRequest<TaskDto>
    {
        public int TaskId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int AssignedToUnitId { get; set; }
        public string AssignedToUserId { get; set; }
        public string SupervisorUserId { get; set; }
        public string CooperatingUnitsIds { get; set; }
        public int ActionTypeId { get; set; }
        public int ProgressPercentage { get; set; }
        public DateTime? DueDate { get; set; }
        public string UpdatedBy { get; set; }

        public UpdateTaskCommand(int taskId, CreateTaskDto dto, string updatedBy)
        {
            TaskId = taskId;
            Title = dto.Title;
            Description = dto.Description;
            AssignedToUnitId = dto.AssignedToUnitId;
            AssignedToUserId = dto.AssignedToUserId;
            SupervisorUserId = dto.SupervisorUserId;
            CooperatingUnitsIds = dto.CooperatingUnitsIds;
            ActionTypeId = dto.ActionTypeId;
            ProgressPercentage = 0; // مقدار اولیه
            DueDate = dto.DueDate;
            UpdatedBy = updatedBy;
        }
    }

    /// <summary>
    /// Command برای حذف وظیفه
    /// </summary>
    public class DeleteTaskCommand : IRequest<bool>
    {
        public int TaskId { get; set; }
        public string DeletedBy { get; set; }

        public DeleteTaskCommand(int taskId, string deletedBy)
        {
            TaskId = taskId;
            DeletedBy = deletedBy;
        }
    }

    /// <summary>
    /// Command برای به‌روزرسانی درصد پیشرفت
    /// </summary>
    public class UpdateTaskProgressCommand : IRequest<TaskDto>
    {
        public int TaskId { get; set; }
        public int ProgressPercentage { get; set; }
        public string CalculationMethod { get; set; } // "Manual" یا "Automatic"
        public string UpdatedBy { get; set; }

        public UpdateTaskProgressCommand(int taskId, int progressPercentage, string calculationMethod, string updatedBy)
        {
            TaskId = taskId;
            ProgressPercentage = progressPercentage;
            CalculationMethod = calculationMethod;
            UpdatedBy = updatedBy;
        }
    }

    /// <summary>
    /// Command برای تغییر وضعیت وظیفه
    /// </summary>
    public class UpdateTaskStatusCommand : IRequest<TaskDto>
    {
        public int TaskId { get; set; }
        public int StatusId { get; set; }
        public string UpdatedBy { get; set; }

        public UpdateTaskStatusCommand(int taskId, int statusId, string updatedBy)
        {
            TaskId = taskId;
            StatusId = statusId;
            UpdatedBy = updatedBy;
        }
    }
}
