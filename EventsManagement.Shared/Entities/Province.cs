namespace EventsManagement.Shared.Entities
{
    /// <summary>
    /// کلاس استان (سطح اول تقسیمات جغرافیایی)
    /// </summary>
    public class Province : BaseEntity
    {
        /// <summary>
        /// نام استان
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// کد استان
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// مناطق تابع این استان
        /// </summary>
        public virtual ICollection<Region> Regions { get; set; } = new List<Region>();
    }
}
