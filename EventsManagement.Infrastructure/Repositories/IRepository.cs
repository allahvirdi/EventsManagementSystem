using EventsManagement.Shared.Entities;

namespace EventsManagement.Infrastructure.Repositories
{
    /// <summary>
    /// اینترفیس Repository عمومی
    /// </summary>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// دریافت تمام رکوردها
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// دریافت رکوردی بر اساس شناسه
        /// </summary>
        Task<T> GetByIdAsync(int id);

        /// <summary>
        /// افزودن رکورد جدید
        /// </summary>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// به‌روزرسانی رکورد
        /// </summary>
        Task<T> UpdateAsync(T entity);

        /// <summary>
        /// حذف رکورد
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// جستجو بر اساس شرط
        /// </summary>
        IQueryable<T> AsQueryable();

        /// <summary>
        /// آیا رکوردی با این شناسه وجود دارد
        /// </summary>
        Task<bool> ExistsAsync(int id);

        /// <summary>
        /// تعداد کل رکوردها
        /// </summary>
        Task<int> CountAsync();

        /// <summary>
        /// ذخیره تغییرات
        /// </summary>
        Task<int> SaveChangesAsync();
    }
}
