using Microsoft.AspNetCore.Identity;

namespace EventsManagement.Shared.Entities
{
    /// <summary>
    /// کلاس کاربر سامانه
    /// </summary>
    public class AppUser : IdentityUser
    {
        /// <summary>
        /// نام کامل کاربر
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// شناسه واحد متعلق به
        /// </summary>
        public int? OrganizationUnitId { get; set; }

        /// <summary>
        /// واحد سازمانی متعلق
        /// </summary>
        public virtual OrganizationUnit OrganizationUnit { get; set; }

        /// <summary>
        /// تاریخ آخرین ورود
        /// </summary>
        public DateTime? LastLoginDate { get; set; }

        /// <summary>
        /// آیا فعال است
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// تاریخ ایجاد
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// تاریخ آخرین تغییر
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}
