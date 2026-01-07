namespace EventsManagement.Shared.DTOs
{
    /// <summary>
    /// DTO برای وظایف
    /// </summary>
    public class CreateTaskDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int EventId { get; set; }
        public int AssignedToUnitId { get; set; }
        public string AssignedToUserId { get; set; }
        public string SupervisorUserId { get; set; }
        public string CooperatingUnitsIds { get; set; } // JSON
        public int ActionTypeId { get; set; }
        public DateTime? DueDate { get; set; }
    }

    /// <summary>
    /// DTO برای نمایش وظیفه
    /// </summary>
    public class TaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int EventId { get; set; }
        public int AssignedToUnitId { get; set; }
        public string AssignedToUserId { get; set; }
        public string SupervisorUserId { get; set; }
        public string CooperatingUnitsIds { get; set; }
        public int ActionTypeId { get; set; }
        public int ProgressPercentage { get; set; }
        public string ProgressCalculationMethod { get; set; }
        public DateTime? DueDate { get; set; }
        public int StatusId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
