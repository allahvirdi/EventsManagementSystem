using Microsoft.EntityFrameworkCore;
using EventsManagement.Infrastructure.Data;
using EventsManagement.Shared.Entities;

namespace EventsManagement.Infrastructure.Repositories
{
    /// <summary>
    /// اینترفیس Repository واحدهای سازمانی
    /// </summary>
    public interface IOrganizationUnitRepository : IRepository<OrganizationUnit>
    {
        /// <summary>
        /// دریافت واحدهای فعال
        /// </summary>
        Task<IEnumerable<OrganizationUnit>> GetActiveUnitsAsync();

        /// <summary>
        /// دریافت واحدهای تابع یک واحد پدر
        /// </summary>
        Task<IEnumerable<OrganizationUnit>> GetChildUnitsAsync(int parentUnitId);

        /// <summary>
        /// دریافت سلسله‌مراتب یک واحد
        /// </summary>
        Task<OrganizationUnit> GetHierarchyAsync(int unitId);

        /// <summary>
        /// جستجو واحدها
        /// </summary>
        Task<IEnumerable<OrganizationUnit>> SearchUnitsAsync(string keyword);
    }

    /// <summary>
    /// پیاده‌سازی Repository واحدهای سازمانی
    /// </summary>
    public class OrganizationUnitRepository : Repository<OrganizationUnit>, IOrganizationUnitRepository
    {
        private readonly ApplicationDbContext _context;

        public OrganizationUnitRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// دریافت واحدهای فعال
        /// </summary>
        public async Task<IEnumerable<OrganizationUnit>> GetActiveUnitsAsync()
        {
            return await _context.OrganizationUnits
                .Where(ou => ou.IsActive && !ou.IsDeleted)
                .OrderBy(ou => ou.Name)
                .ToListAsync();
        }

        /// <summary>
        /// دریافت واحدهای تابع یک واحد پدر
        /// </summary>
        public async Task<IEnumerable<OrganizationUnit>> GetChildUnitsAsync(int parentUnitId)
        {
            return await _context.OrganizationUnits
                .Where(ou => ou.ParentUnitId == parentUnitId && !ou.IsDeleted)
                .OrderBy(ou => ou.Name)
                .ToListAsync();
        }

        /// <summary>
        /// دریافت سلسله‌مراتب یک واحد
        /// </summary>
        public async Task<OrganizationUnit> GetHierarchyAsync(int unitId)
        {
            return await _context.OrganizationUnits
                .Include(ou => ou.ChildUnits)
                .Include(ou => ou.Users)
                .FirstOrDefaultAsync(ou => ou.Id == unitId && !ou.IsDeleted);
        }

        /// <summary>
        /// جستجو واحدها
        /// </summary>
        public async Task<IEnumerable<OrganizationUnit>> SearchUnitsAsync(string keyword)
        {
            return await _context.OrganizationUnits
                .Where(ou => ou.Name.Contains(keyword) && !ou.IsDeleted)
                .OrderBy(ou => ou.Name)
                .ToListAsync();
        }
    }
}
