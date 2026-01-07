namespace EventsManagement.Shared.Entities
{
    /// <summary>
    /// کلاس پایه برای تمام موجودیت‌های سامانه
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// شناسه منحصر به فرد
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// تاریخ ایجاد
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// شناسه کاربر ایجادکننده
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// تاریخ آخرین تغییر
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// شناسه کاربر ویرایشکننده
        /// </summary>
        public string UpdatedBy { get; set; }

        /// <summary>
        /// آیا حذف شده است
        /// </summary>
        public bool IsDeleted { get; set; } = false;
    }
}
