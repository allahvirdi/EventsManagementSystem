namespace EventsManagement.Shared.Entities
{
    /// <summary>
    /// کلاس واحد سازمانی/واحد اقدام کننده
    /// </summary>
    public class OrganizationUnit : BaseEntity
    {
        /// <summary>
        /// نام واحد
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// نوع واحد (داخلی/خارجی)
        /// </summary>
        public string UnitType { get; set; } // "Internal" یا "External"

        /// <summary>
        /// کد واحد
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// شناسه واحد پدر (برای سلسله‌مراتب)
        /// </summary>
        public int? ParentUnitId { get; set; }

        /// <summary>
        /// واحد پدر
        /// </summary>
        public virtual OrganizationUnit ParentUnit { get; set; }

        /// <summary>
        /// واحدهای زیرمجموعه
        /// </summary>
        public virtual ICollection<OrganizationUnit> ChildUnits { get; set; } = new List<OrganizationUnit>();

        /// <summary>
        /// شناسه شهر
        /// </summary>
        public int? ProvinceId { get; set; }

        /// <summary>
        /// شناسه منطقه
        /// </summary>
        public int? RegionId { get; set; }

        /// <summary>
        /// شناسه مدرسه (در صورت وابستگی)
        /// </summary>
        public int? SchoolId { get; set; }

        /// <summary>
        /// آدرس
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// شماره تماس
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// کاربران این واحد
        /// </summary>
        public virtual ICollection<AppUser> Users { get; set; } = new List<AppUser>();

        /// <summary>
        /// آیا فعال است
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}
