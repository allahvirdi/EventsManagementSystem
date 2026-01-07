namespace EventsManagement.Shared.Entities
{
    /// <summary>
    /// کلاس رویداد/رخداد
    /// </summary>
    public class Event : BaseEntity
    {
        /// <summary>
        /// عنوان رویداد
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// شرح تفصیلی رویداد
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// شناسه منشا رویداد (از جدول DynamicTable)
        /// </summary>
        public int EventSourceId { get; set; }

        /// <summary>
        /// شناسه موضوع رویداد (از جدول DynamicTable)
        /// </summary>
        public int EventSubjectId { get; set; }

        /// <summary>
        /// شناسه فوریت رویداد (از جدول DynamicTable)
        /// </summary>
        public int UrgencyId { get; set; }

        /// <summary>
        /// شناسه گستره وقوع (از جدول DynamicTable)
        /// </summary>
        public int ScopeId { get; set; }

        /// <summary>
        /// جزئیات گستره (استان/منطقه/مدرسه)
        /// </summary>
        public string ScopeDetails { get; set; } // JSON format

        /// <summary>
        /// شناسه گستره اثر (از جدول DynamicTable)
        /// </summary>
        public int ImpactScopeId { get; set; }

        /// <summary>
        /// جزئیات گستره اثر (استان/منطقه/مدرسه)
        /// </summary>
        public string ImpactScopeDetails { get; set; } // JSON format

        /// <summary>
        /// شناسه محدوده اثر (درون/برون دستگاهی)
        /// </summary>
        public int ImpactRangeId { get; set; }

        /// <summary>
        /// تاریخ شروع رخداد
        /// </summary>
        public DateTime EventStartDate { get; set; }

        /// <summary>
        /// تاریخ پایان رخداد
        /// </summary>
        public DateTime? EventEndDate { get; set; }

        /// <summary>
        /// شناسه واحد اقدام‌کننده (از OrganizationUnit)
        /// </summary>
        public int ActionUnitId { get; set; }

        /// <summary>
        /// واحد اقدام‌کننده
        /// </summary>
        public virtual OrganizationUnit ActionUnit { get; set; }

        /// <summary>
        /// شناسه وضعیت (از جدول DynamicTable)
        /// </summary>
        public int StatusId { get; set; }

        /// <summary>
        /// شناسه کاربر ثبت‌کننده
        /// </summary>
        public string? RegisteredBy { get; set; }

        /// <summary>
        /// شناسه کاربر بررسی‌کننده/مرجع بالاتر
        /// </summary>
        public string? ReviewedBy { get; set; }

        /// <summary>
        /// وظایف مرتبط با این رویداد
        /// </summary>
        public virtual ICollection<EventTask> Tasks { get; set; } = new List<EventTask>();

        /// <summary>
        /// مستندات مرتبط
        /// </summary>
        public virtual ICollection<DocumentMetadata> Documents { get; set; } = new List<DocumentMetadata>();

        /// <summary>
        /// رویدادهای مرتبط
        /// </summary>
        public virtual ICollection<EventRelationship> RelatedEvents { get; set; } = new List<EventRelationship>();

        /// <summary>
        /// یادداشت‌ها/نظرات
        /// </summary>
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
