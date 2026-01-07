using Microsoft.EntityFrameworkCore;
using EventsManagement.Infrastructure.Data;
using EventsManagement.Shared.Entities;

namespace EventsManagement.Infrastructure.Repositories
{
    /// <summary>
    /// پیاده‌سازی عمومی Repository
    /// </summary>
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        /// <summary>
        /// دریافت تمام رکوردها
        /// </summary>
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// دریافت رکوردی بر اساس شناسه
        /// </summary>
        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        /// <summary>
        /// افزودن رکورد جدید
        /// </summary>
        public async Task<T> AddAsync(T entity)
        {
            // تنظیم CreatedAt برای BaseEntity
            if (entity is BaseEntity baseEntity)
            {
                baseEntity.CreatedAt = DateTime.Now;
            }

            await _dbSet.AddAsync(entity);
            await SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// به‌روزرسانی رکورد
        /// </summary>
        public async Task<T> UpdateAsync(T entity)
        {
            // تنظیم UpdatedAt برای BaseEntity
            if (entity is BaseEntity baseEntity)
            {
                baseEntity.UpdatedAt = DateTime.Now;
            }

            _dbSet.Update(entity);
            await SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// حذف رکورد
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null)
                return false;

            // نرم‌حذف برای موجودیت‌های با IsDeleted
            if (entity is BaseEntity baseEntity)
            {
                baseEntity.IsDeleted = true;
                await UpdateAsync(entity);
            }
            else
            {
                _dbSet.Remove(entity);
                await SaveChangesAsync();
            }

            return true;
        }

        /// <summary>
        /// جستجو بر اساس شرط
        /// </summary>
        public IQueryable<T> AsQueryable()
        {
            return _dbSet.AsQueryable();
        }

        /// <summary>
        /// آیا رکوردی با این شناسه وجود دارد
        /// </summary>
        public async Task<bool> ExistsAsync(int id)
        {
            return await _dbSet.FindAsync(id) != null;
        }

        /// <summary>
        /// تعداد کل رکوردها
        /// </summary>
        public async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }

        /// <summary>
        /// ذخیره تغییرات
        /// </summary>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
