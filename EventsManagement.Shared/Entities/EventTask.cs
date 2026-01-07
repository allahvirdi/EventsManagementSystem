namespace EventsManagement.Shared.Entities
{
    /// <summary>
    /// کلاس وظیفه/اقدام (Task)
    /// </summary>
    public class EventTask : BaseEntity
    {
        /// <summary>
        /// عنوان وظیفه
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// شرح تفصیلی وظیفه
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// شناسه رویداد مرتبط
        /// </summary>
        public int EventId { get; set; }

        /// <summary>
        /// رویداد مرتبط
        /// </summary>
        public virtual Event Event { get; set; }

        /// <summary>
        /// شناسه واحد پیشنهادی برای اقدام
        /// </summary>
        public int AssignedToUnitId { get; set; }

        /// <summary>
        /// واحد پیشنهادی
        /// </summary>
        public virtual OrganizationUnit AssignedToUnit { get; set; }

        /// <summary>
        /// شناسه کاربر پیشنهادی
        /// </summary>
        public string AssignedToUserId { get; set; }

        /// <summary>
        /// شناسه هادی/پیگیری‌کننده
        /// </summary>
        public string SupervisorUserId { get; set; }

        /// <summary>
        /// واحدهای همکار (JSON format با شناسه‌های واحد)
        /// </summary>
        public string CooperatingUnitsIds { get; set; }

        /// <summary>
        /// شناسه نوع اقدام (از جدول DynamicTable)
        /// </summary>
        public int ActionTypeId { get; set; }

        /// <summary>
        /// درصد پیشرفت
        /// </summary>
        public int ProgressPercentage { get; set; } = 0;

        /// <summary>
        /// روش محاسبه پیشرفت (خودکار/دستی)
        /// </summary>
        public string ProgressCalculationMethod { get; set; } = "Manual"; // "Automatic" یا "Manual"

        /// <summary>
        /// تاریخ پایان مورد انتظار
        /// </summary>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// شناسه وضعیت
        /// </summary>
        public int StatusId { get; set; }

        /// <summary>
        /// پاسخ‌های مرتبط
        /// </summary>
        public virtual ICollection<TaskReply> Replies { get; set; } = new List<TaskReply>();

        /// <summary>
        /// مستندات مرتبط
        /// </summary>
        public virtual ICollection<DocumentMetadata> Documents { get; set; } = new List<DocumentMetadata>();
    }
}
