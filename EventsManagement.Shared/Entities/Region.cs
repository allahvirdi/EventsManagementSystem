namespace EventsManagement.Shared.Entities
{
    /// <summary>
    /// کلاس منطقه (سطح دوم تقسیمات جغرافیایی)
    /// </summary>
    public class Region : BaseEntity
    {
        /// <summary>
        /// نام منطقه
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// کد منطقه
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// شناسه استان تابع
        /// </summary>
        public int ProvinceId { get; set; }

        /// <summary>
        /// استان تابع
        /// </summary>
        public virtual Province Province { get; set; }

        /// <summary>
        /// مدارس تابع این منطقه
        /// </summary>
        public virtual ICollection<School> Schools { get; set; } = new List<School>();
    }
}
