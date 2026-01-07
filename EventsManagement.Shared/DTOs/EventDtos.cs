namespace EventsManagement.Shared.DTOs
{
    /// <summary>
    /// DTO برای رویداد
    /// </summary>
    public class CreateEventDto
    {
        /// <summary>
        /// عنوان رویداد
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// شرح تفصیلی
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// شناسه منشا
        /// </summary>
        public int EventSourceId { get; set; }

        /// <summary>
        /// شناسه موضوع
        /// </summary>
        public int EventSubjectId { get; set; }

        /// <summary>
        /// شناسه فوریت
        /// </summary>
        public int UrgencyId { get; set; }

        /// <summary>
        /// شناسه گستره وقوع
        /// </summary>
        public int ScopeId { get; set; }

        /// <summary>
        /// جزئیات گستره (JSON)
        /// </summary>
        public string ScopeDetails { get; set; }

        /// <summary>
        /// شناسه گستره اثر
        /// </summary>
        public int ImpactScopeId { get; set; }

        /// <summary>
        /// جزئیات گستره اثر (JSON)
        /// </summary>
        public string ImpactScopeDetails { get; set; }

        /// <summary>
        /// شناسه محدوده اثر
        /// </summary>
        public int ImpactRangeId { get; set; }

        /// <summary>
        /// تاریخ شروع
        /// </summary>
        public DateTime EventStartDate { get; set; }

        /// <summary>
        /// تاریخ پایان
        /// </summary>
        public DateTime? EventEndDate { get; set; }

        /// <summary>
        /// شناسه واحد اقدام‌کننده
        /// </summary>
        public int ActionUnitId { get; set; }
    }

    /// <summary>
    /// DTO برای نمایش رویداد
    /// </summary>
    public class EventDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int EventSourceId { get; set; }
        public int EventSubjectId { get; set; }
        public int UrgencyId { get; set; }
        public int ScopeId { get; set; }
        public string ScopeDetails { get; set; }
        public int ImpactScopeId { get; set; }
        public string ImpactScopeDetails { get; set; }
        public int ImpactRangeId { get; set; }
        public DateTime EventStartDate { get; set; }
        public DateTime? EventEndDate { get; set; }
        public int ActionUnitId { get; set; }
        public int StatusId { get; set; }
        public string RegisteredBy { get; set; }
        public string ReviewedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
