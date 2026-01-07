namespace EventsManagement.Shared.Entities
{
    /// <summary>
    /// کلاس برای ثبت فعالیت‌های کاربران
    /// </summary>
    public class UserActivityLog : BaseEntity
    {
        /// <summary>
        /// شناسه کاربر
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// نوع فعالیت (Login, Logout, CreateEvent, UpdateEvent, etc)
        /// </summary>
        public string ActivityType { get; set; }

        /// <summary>
        /// شرح فعالیت
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// IP آدرس
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// User Agent
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// آیا موفقیت‌آمیز بوده است
        /// </summary>
        public bool IsSuccessful { get; set; } = true;

        /// <summary>
        /// پیام خطا (در صورت ناکامی)
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// تاریخ و ساعت فعالیت
        /// </summary>
        public DateTime ActivityDateTime { get; set; } = DateTime.Now;
    }
}
