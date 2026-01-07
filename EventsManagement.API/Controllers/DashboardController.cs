using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using EventsManagement.Shared.DTOs;
using EventsManagement.Infrastructure.Data;
using Serilog;

namespace EventsManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// دریافت آمار داشبورد
        /// </summary>
        [HttpGet("stats")]
        public async Task<ActionResult<ApiResult<DashboardStats>>> GetDashboardStats()
        {
            try
            {
                var stats = new DashboardStats
                {
                    TotalEvents = await _context.Events.CountAsync(),
                    TotalTasks = await _context.EventTasks.CountAsync(),
                    TotalUsers = await _context.Users.CountAsync(),
                    
                    EventsByStatus = await _context.Events
                        .GroupBy(e => e.StatusId)
                        .Select(g => new StatusCount { StatusId = g.Key, Count = g.Count() })
                        .ToListAsync(),
                    
                    TasksByStatus = await _context.EventTasks
                        .GroupBy(t => t.StatusId)
                        .Select(g => new StatusCount { StatusId = g.Key, Count = g.Count() })
                        .ToListAsync(),
                    
                    AverageProgress = await _context.EventTasks.AverageAsync(t => (double?)t.ProgressPercentage) ?? 0,
                    
                    RecentEvents = await _context.Events
                        .OrderByDescending(e => e.CreatedAt)
                        .Take(5)
                        .Select(e => new RecentEventDto
                        {
                            Id = e.Id,
                            Title = e.Title,
                            StatusId = e.StatusId,
                            CreatedAt = e.CreatedAt
                        })
                        .ToListAsync()
                };

                return Ok(ApiResult<DashboardStats>.Success(stats, "آمار با موفقیت دریافت شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "خطا در دریافت آمار داشبورد");
                return Ok(ApiResult<DashboardStats>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// دریافت لاگ فعالیت کاربران
        /// </summary>
        [HttpGet("activity-logs")]
        public async Task<ActionResult<ApiResult<List<UserActivityLogDto>>>> GetActivityLogs(
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var logs = await _context.UserActivityLogs
                    .OrderByDescending(l => l.ActivityDateTime)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(l => new UserActivityLogDto
                    {
                        Id = l.Id,
                        UserId = l.UserId,
                        ActivityType = l.ActivityType,
                        Description = l.Description,
                        IpAddress = l.IpAddress,
                        IsSuccessful = l.IsSuccessful,
                        ActivityDateTime = l.ActivityDateTime
                    })
                    .ToListAsync();

                return Ok(ApiResult<List<UserActivityLogDto>>.Success(logs, $"{logs.Count} لاگ یافت شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "خطا در دریافت لاگ فعالیت کاربران");
                return Ok(ApiResult<List<UserActivityLogDto>>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }
    }

    // DTOs
    public class DashboardStats
    {
        public int TotalEvents { get; set; }
        public int TotalTasks { get; set; }
        public int TotalUsers { get; set; }
        public List<StatusCount> EventsByStatus { get; set; } = new();
        public List<StatusCount> TasksByStatus { get; set; } = new();
        public double AverageProgress { get; set; }
        public List<RecentEventDto> RecentEvents { get; set; } = new();
    }

    public class StatusCount
    {
        public int StatusId { get; set; }
        public int Count { get; set; }
    }

    public class RecentEventDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int StatusId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UserActivityLogDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string ActivityType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public bool IsSuccessful { get; set; }
        public DateTime ActivityDateTime { get; set; }
    }
}
