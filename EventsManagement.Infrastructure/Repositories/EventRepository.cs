using Microsoft.EntityFrameworkCore;
using EventsManagement.Infrastructure.Data;
using EventsManagement.Shared.Entities;

namespace EventsManagement.Infrastructure.Repositories
{
    /// <summary>
    /// اینترفیس Repository مخصوص رویدادها
    /// </summary>
    public interface IEventRepository : IRepository<Event>
    {
        /// <summary>
        /// دریافت رویدادها بر اساس واحد
        /// </summary>
        Task<IEnumerable<Event>> GetEventsByUnitAsync(int unitId);

        /// <summary>
        /// دریافت رویدادها بر اساس وضعیت
        /// </summary>
        Task<IEnumerable<Event>> GetEventsByStatusAsync(int statusId);

        /// <summary>
        /// جستجو رویدادها
        /// </summary>
        Task<IEnumerable<Event>> SearchEventsAsync(string keyword);

        /// <summary>
        /// دریافت رویداد با تمام وابستگی‌ها
        /// </summary>
        Task<Event> GetEventWithDetailsAsync(int eventId);
    }

    /// <summary>
    /// پیاده‌سازی Repository رویدادها
    /// </summary>
    public class EventRepository : Repository<Event>, IEventRepository
    {
        private readonly ApplicationDbContext _context;

        public EventRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// دریافت رویدادها بر اساس واحد
        /// </summary>
        public async Task<IEnumerable<Event>> GetEventsByUnitAsync(int unitId)
        {
            return await _context.Events
                .Where(e => e.ActionUnitId == unitId && !e.IsDeleted)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// دریافت رویدادها بر اساس وضعیت
        /// </summary>
        public async Task<IEnumerable<Event>> GetEventsByStatusAsync(int statusId)
        {
            return await _context.Events
                .Where(e => e.StatusId == statusId && !e.IsDeleted)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// جستجو رویدادها
        /// </summary>
        public async Task<IEnumerable<Event>> SearchEventsAsync(string keyword)
        {
            return await _context.Events
                .Where(e => (e.Title.Contains(keyword) || e.Description.Contains(keyword)) && !e.IsDeleted)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// دریافت رویداد با تمام وابستگی‌ها
        /// </summary>
        public async Task<Event> GetEventWithDetailsAsync(int eventId)
        {
            return await _context.Events
                .Include(e => e.ActionUnit)
                .Include(e => e.Tasks)
                .Include(e => e.Documents)
                .Include(e => e.Comments)
                .Include(e => e.RelatedEvents)
                .FirstOrDefaultAsync(e => e.Id == eventId && !e.IsDeleted);
        }
    }
}
