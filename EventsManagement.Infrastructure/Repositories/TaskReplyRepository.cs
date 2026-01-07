using Microsoft.EntityFrameworkCore;
using EventsManagement.Infrastructure.Data;
using EventsManagement.Shared.Entities;

namespace EventsManagement.Infrastructure.Repositories
{
    /// <summary>
    /// اینترفیس Repository پاسخ‌های وظیفه
    /// </summary>
    public interface ITaskReplyRepository : IRepository<TaskReply>
    {
        /// <summary>
        /// دریافت پاسخ‌های یک وظیفه
        /// </summary>
        Task<IEnumerable<TaskReply>> GetRepliesByTaskAsync(int taskId);

        /// <summary>
        /// دریافت پاسخ با جزئیات
        /// </summary>
        Task<TaskReply> GetReplyWithDetailsAsync(int replyId);

        /// <summary>
        /// دریافت آخرین پاسخ برای وظیفه
        /// </summary>
        Task<TaskReply> GetLatestReplyAsync(int taskId);
    }

    /// <summary>
    /// پیاده‌سازی Repository پاسخ‌های وظیفه
    /// </summary>
    public class TaskReplyRepository : Repository<TaskReply>, ITaskReplyRepository
    {
        private readonly ApplicationDbContext _context;

        public TaskReplyRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// دریافت پاسخ‌های یک وظیفه
        /// </summary>
        public async Task<IEnumerable<TaskReply>> GetRepliesByTaskAsync(int taskId)
        {
            return await _context.TaskReplies
                .Where(r => r.TaskId == taskId && !r.IsDeleted)
                .OrderByDescending(r => r.RegisteredDate)
                .ToListAsync();
        }

        /// <summary>
        /// دریافت پاسخ با جزئیات
        /// </summary>
        public async Task<TaskReply> GetReplyWithDetailsAsync(int replyId)
        {
            return await _context.TaskReplies
                .Include(r => r.Task)
                .Include(r => r.Documents)
                .FirstOrDefaultAsync(r => r.Id == replyId && !r.IsDeleted);
        }

        /// <summary>
        /// دریافت آخرین پاسخ برای وظیفه
        /// </summary>
        public async Task<TaskReply> GetLatestReplyAsync(int taskId)
        {
            return await _context.TaskReplies
                .Where(r => r.TaskId == taskId && !r.IsDeleted)
                .OrderByDescending(r => r.RegisteredDate)
                .FirstOrDefaultAsync();
        }
    }
}
