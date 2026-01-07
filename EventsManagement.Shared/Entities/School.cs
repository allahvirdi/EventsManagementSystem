namespace EventsManagement.Shared.Entities
{
    /// <summary>
    /// کلاس مدرسه (سطح سوم تقسیمات جغرافیایی)
    /// </summary>
    public class School : BaseEntity
    {
        /// <summary>
        /// نام مدرسه
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// کد مدرسه
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// شناسه منطقه تابع
        /// </summary>
        public int? RegionId { get; set; }

        /// <summary>
        /// منطقه تابع
        /// </summary>
        public virtual Region Region { get; set; }

        /// <summary>
        /// شناسه استان تابع (در صورت مستقیم تحت استان بودن)
        /// </summary>
        public int? ProvinceId { get; set; }

        /// <summary>
        /// استان تابع
        /// </summary>
        public virtual Province Province { get; set; }

        /// <summary>
        /// آدرس
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// شماره تماس
        /// </summary>
        public string PhoneNumber { get; set; }
    }
}
