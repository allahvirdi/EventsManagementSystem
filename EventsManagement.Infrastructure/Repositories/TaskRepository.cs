using Microsoft.EntityFrameworkCore;
using EventsManagement.Infrastructure.Data;
using EventsManagement.Shared.Entities;

namespace EventsManagement.Infrastructure.Repositories
{
    /// <summary>
    /// اینترفیس Repository وظایف
    /// </summary>
    public interface ITaskRepository : IRepository<EventTask>
    {
        /// <summary>
        /// دریافت وظایف یک رویداد
        /// </summary>
        Task<IEnumerable<EventTask>> GetTasksByEventAsync(int eventId);

        /// <summary>
        /// دریافت وظایف اختصاص‌یافته به واحد
        /// </summary>
        Task<IEnumerable<EventTask>> GetTasksByUnitAsync(int unitId);

        /// <summary>
        /// دریافت وظیفه با جزئیات
        /// </summary>
        Task<EventTask> GetTaskWithDetailsAsync(int taskId);

        /// <summary>
        /// دریافت وظایف بر اساس وضعیت
        /// </summary>
        Task<IEnumerable<EventTask>> GetTasksByStatusAsync(int statusId);
    }

    /// <summary>
    /// پیاده‌سازی Repository وظایف
    /// </summary>
    public class TaskRepository : Repository<EventTask>, ITaskRepository
    {
        private readonly ApplicationDbContext _context;

        public TaskRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// دریافت وظایف یک رویداد
        /// </summary>
        public async Task<IEnumerable<EventTask>> GetTasksByEventAsync(int eventId)
        {
            return await _context.EventTasks
                .Where(t => t.EventId == eventId && !t.IsDeleted)
                .Include(t => t.AssignedToUnit)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// دریافت وظایف اختصاص‌یافته به واحد
        /// </summary>
        public async Task<IEnumerable<EventTask>> GetTasksByUnitAsync(int unitId)
        {
            return await _context.EventTasks
                .Where(t => t.AssignedToUnitId == unitId && !t.IsDeleted)
                .Include(t => t.Event)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// دریافت وظیفه با جزئیات
        /// </summary>
        public async Task<EventTask> GetTaskWithDetailsAsync(int taskId)
        {
            return await _context.EventTasks
                .Include(t => t.Event)
                .Include(t => t.AssignedToUnit)
                .Include(t => t.Replies)
                .Include(t => t.Documents)
                .FirstOrDefaultAsync(t => t.Id == taskId && !t.IsDeleted);
        }

        /// <summary>
        /// دریافت وظایف بر اساس وضعیت
        /// </summary>
        public async Task<IEnumerable<EventTask>> GetTasksByStatusAsync(int statusId)
        {
            return await _context.EventTasks
                .Where(t => t.StatusId == statusId && !t.IsDeleted)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
    }
}
