namespace EventsManagement.Shared.Entities
{
    /// <summary>
    /// کلاس پاسخ/نتیجه وظیفه
    /// </summary>
    public class TaskReply : BaseEntity
    {
        /// <summary>
        /// شناسه وظیفه
        /// </summary>
        public int TaskId { get; set; }

        /// <summary>
        /// وظیفه
        /// </summary>
        public virtual EventTask Task { get; set; }

        /// <summary>
        /// محتوای پاسخ
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// تاریخ و ساعت اقدام (وارد شده توسط کاربر)
        /// </summary>
        public DateTime ActionDateTime { get; set; }

        /// <summary>
        /// شناسه کاربر پاسخ‌دهنده
        /// </summary>
        public string RespondedByUserId { get; set; }

        /// <summary>
        /// تاریخ ثبت (خودکار)
        /// </summary>
        public DateTime RegisteredDate { get; set; } = DateTime.Now;

        /// <summary>
        /// شناسه هادی/پیگیری‌کننده (از OrganizationUnit)
        /// </summary>
        public int? SupervisorUnitId { get; set; }

        /// <summary>
        /// مستندات مرتبط
        /// </summary>
        public virtual ICollection<DocumentMetadata> Documents { get; set; } = new List<DocumentMetadata>();
    }
}
