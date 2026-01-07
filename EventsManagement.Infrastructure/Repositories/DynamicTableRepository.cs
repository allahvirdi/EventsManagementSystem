using Microsoft.EntityFrameworkCore;
using EventsManagement.Infrastructure.Data;
using EventsManagement.Shared.Entities;

namespace EventsManagement.Infrastructure.Repositories
{
    /// <summary>
    /// اینترفیس Repository جداول پویا
    /// </summary>
    public interface IDynamicTableRepository : IRepository<DynamicTable>
    {
        /// <summary>
        /// دریافت مقادیر جدول خاص
        /// </summary>
        Task<IEnumerable<DynamicTable>> GetByTableNameAsync(string tableName);

        /// <summary>
        /// دریافت کد بر اساس نام جدول و مقدار
        /// </summary>
        Task<DynamicTable> GetByTableNameAndValueAsync(string tableName, string value);

        /// <summary>
        /// دریافت تمام جدول‌های فعال
        /// </summary>
        Task<IEnumerable<DynamicTable>> GetActiveTablesAsync();

        /// <summary>
        /// دریافت زیرمجموعه‌های یک رکورد
        /// </summary>
        Task<IEnumerable<DynamicTable>> GetChildrenAsync(int parentId);
    }

    /// <summary>
    /// پیاده‌سازی Repository جداول پویا
    /// </summary>
    public class DynamicTableRepository : Repository<DynamicTable>, IDynamicTableRepository
    {
        private readonly ApplicationDbContext _context;

        public DynamicTableRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// دریافت مقادیر جدول خاص
        /// </summary>
        public async Task<IEnumerable<DynamicTable>> GetByTableNameAsync(string tableName)
        {
            return await _context.DynamicTables
                .Where(dt => dt.TableName == tableName && dt.IsActive && !dt.IsDeleted)
                .OrderBy(dt => dt.DisplayOrder)
                .ToListAsync();
        }

        /// <summary>
        /// دریافت کد بر اساس نام جدول و مقدار
        /// </summary>
        public async Task<DynamicTable> GetByTableNameAndValueAsync(string tableName, string value)
        {
            return await _context.DynamicTables
                .FirstOrDefaultAsync(dt => dt.TableName == tableName && dt.Value == value && !dt.IsDeleted);
        }

        /// <summary>
        /// دریافت تمام جدول‌های فعال
        /// </summary>
        public async Task<IEnumerable<DynamicTable>> GetActiveTablesAsync()
        {
            return await _context.DynamicTables
                .Where(dt => dt.IsActive && !dt.IsDeleted)
                .OrderBy(dt => dt.TableName)
                .ThenBy(dt => dt.DisplayOrder)
                .ToListAsync();
        }

        /// <summary>
        /// دریافت زیرمجموعه‌های یک رکورد
        /// </summary>
        public async Task<IEnumerable<DynamicTable>> GetChildrenAsync(int parentId)
        {
            return await _context.DynamicTables
                .Where(dt => dt.ParentId == parentId && !dt.IsDeleted)
                .OrderBy(dt => dt.DisplayOrder)
                .ToListAsync();
        }
    }
}
