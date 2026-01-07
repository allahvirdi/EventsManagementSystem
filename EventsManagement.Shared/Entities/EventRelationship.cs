namespace EventsManagement.Shared.Entities
{
    /// <summary>
    /// کلاس برای ارتباط‌های میان رویدادها
    /// </summary>
    public class EventRelationship : BaseEntity
    {
        /// <summary>
        /// شناسه رویداد اول
        /// </summary>
        public int EventId1 { get; set; }

        /// <summary>
        /// رویداد اول
        /// </summary>
        public virtual Event Event1 { get; set; }

        /// <summary>
        /// شناسه رویداد دوم
        /// </summary>
        public int EventId2 { get; set; }

        /// <summary>
        /// رویداد دوم
        /// </summary>
        public virtual Event Event2 { get; set; }

        /// <summary>
        /// نوع ارتباط (مثال: "پیشنیاز", "نتیجه", "مرتبط")
        /// </summary>
        public string RelationshipType { get; set; }

        /// <summary>
        /// شرح ارتباط
        /// </summary>
        public string Description { get; set; }
    }
}
