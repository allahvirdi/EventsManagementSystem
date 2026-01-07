namespace EventsManagement.Shared.Entities
{
    /// <summary>
    /// کلاس نظرات و یادداشت‌ها
    /// </summary>
    public class Comment : BaseEntity
    {
        /// <summary>
        /// محتوای نظر
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// شناسه رویداد
        /// </summary>
        public int? EventId { get; set; }

        /// <summary>
        /// رویداد
        /// </summary>
        public virtual Event Event { get; set; }

        /// <summary>
        /// شناسه وظیفه
        /// </summary>
        public int? TaskId { get; set; }

        /// <summary>
        /// شناسه کاربر نظر‌دهنده
        /// </summary>
        public string CommentedBy { get; set; }

        /// <summary>
        /// شناسه نظر پدر (در صورت پاسخ به نظر دیگری)
        /// </summary>
        public int? ParentCommentId { get; set; }
    }
}
