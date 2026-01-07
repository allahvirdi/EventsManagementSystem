namespace EventsManagement.Shared.Entities
{
    /// <summary>
    /// کلاس جداول پویا برای ذخیره انواع مختلف داده‌های انتخابی
    /// </summary>
    public class DynamicTable : BaseEntity
    {
        /// <summary>
        /// نام جدول (EventSource, EventSubject, Urgency, Scope, State, ActionType)
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// کد کاربردی (int برای هر جدول)
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// مقدار یا عنوان
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// شرح تفصیلی
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// ترتیب نمایش
        /// </summary>
        public int DisplayOrder { get; set; } = 0;

        /// <summary>
        /// آیا فعال است
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// شناسه جدول پدر (برای سلسله‌مراتب)
        /// </summary>
        public int? ParentId { get; set; }
    }
}
