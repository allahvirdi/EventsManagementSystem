namespace EventsManagement.Shared.DTOs
{
    /// <summary>
    /// DTO برای پاسخ وظیفه
    /// </summary>
    public class CreateTaskReplyDto
    {
        public int TaskId { get; set; }
        public string Content { get; set; }
        public DateTime ActionDateTime { get; set; }
        public int? SupervisorUnitId { get; set; }
    }

    /// <summary>
    /// DTO برای نمایش پاسخ
    /// </summary>
    public class TaskReplyDto
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public string Content { get; set; }
        public DateTime ActionDateTime { get; set; }
        public string RespondedByUserId { get; set; }
        public DateTime RegisteredDate { get; set; }
        public int? SupervisorUnitId { get; set; }
    }
}
