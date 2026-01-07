using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using EventsManagement.Shared.DTOs;
using EventsManagement.Shared.Entities;
using EventsManagement.Infrastructure.Data;
using Serilog;

namespace EventsManagement.API.Controllers
{
    /// <summary>
    /// کنترلر مدیریت جداول پویا (Dropdown Data)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DynamicTablesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DynamicTablesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// دریافت لیست تمام جداول پویا
        /// GET: api/DynamicTables
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResult<List<DynamicTableDto>>>> GetAll()
        {
            try
            {
                var tables = await _context.DynamicTables
                    .Where(t => !t.IsDeleted)
                    .OrderBy(t => t.TableName)
                    .ThenBy(t => t.DisplayOrder)
                    .Select(t => new DynamicTableDto
                    {
                        Id = t.Id,
                        TableName = t.TableName,
                        Code = t.Code,
                        Value = t.Value,
                        ParentId = t.ParentId,
                        DisplayOrder = t.DisplayOrder,
                        IsActive = t.IsActive
                    })
                    .ToListAsync();

                return Ok(ApiResult<List<DynamicTableDto>>.Success(tables, $"{tables.Count} رکورد یافت شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "خطا در دریافت جداول پویا");
                return Ok(ApiResult<List<DynamicTableDto>>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// دریافت رکوردهای یک جدول خاص
        /// GET: api/DynamicTables/ByTable/{tableName}
        /// </summary>
        [HttpGet("ByTable/{tableName}")]
        public async Task<ActionResult<ApiResult<List<DynamicTableDto>>>> GetByTableName(string tableName)
        {
            try
            {
                var tables = await _context.DynamicTables
                    .Where(t => t.TableName == tableName && !t.IsDeleted)
                    .OrderBy(t => t.DisplayOrder)
                    .Select(t => new DynamicTableDto
                    {
                        Id = t.Id,
                        TableName = t.TableName,
                        Code = t.Code,
                        Value = t.Value,
                        ParentId = t.ParentId,
                        DisplayOrder = t.DisplayOrder,
                        IsActive = t.IsActive
                    })
                    .ToListAsync();

                return Ok(ApiResult<List<DynamicTableDto>>.Success(tables, $"{tables.Count} رکورد یافت شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در دریافت جدول {tableName}");
                return Ok(ApiResult<List<DynamicTableDto>>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// دریافت لیست نام جداول منحصربه‌فرد
        /// GET: api/DynamicTables/TableNames
        /// </summary>
        [HttpGet("TableNames")]
        public async Task<ActionResult<ApiResult<List<string>>>> GetTableNames()
        {
            try
            {
                var tableNames = await _context.DynamicTables
                    .Where(t => !t.IsDeleted)
                    .Select(t => t.TableName)
                    .Distinct()
                    .OrderBy(name => name)
                    .ToListAsync();

                return Ok(ApiResult<List<string>>.Success(tableNames, $"{tableNames.Count} جدول یافت شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "خطا در دریافت نام جداول");
                return Ok(ApiResult<List<string>>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// دریافت یک رکورد جدول پویا
        /// GET: api/DynamicTables/{id}
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<DynamicTableDto>>> GetById(int id)
        {
            try
            {
                var table = await _context.DynamicTables
                    .Where(t => t.Id == id && !t.IsDeleted)
                    .Select(t => new DynamicTableDto
                    {
                        Id = t.Id,
                        TableName = t.TableName,
                        Code = t.Code,
                        Value = t.Value,
                        ParentId = t.ParentId,
                        DisplayOrder = t.DisplayOrder,
                        IsActive = t.IsActive
                    })
                    .FirstOrDefaultAsync();

                if (table == null)
                    return Ok(ApiResult<DynamicTableDto>.Failure("رکورد یافت نشد", 404));

                return Ok(ApiResult<DynamicTableDto>.Success(table));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در دریافت رکورد {id}");
                return Ok(ApiResult<DynamicTableDto>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// ایجاد رکورد جدید در جدول پویا
        /// POST: api/DynamicTables
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ApiResult<DynamicTableDto>>> Create([FromBody] CreateDynamicTableDto model)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                // بررسی تکراری بودن
                var exists = await _context.DynamicTables
                    .AnyAsync(t => t.TableName == model.TableName && t.Code == model.Code && !t.IsDeleted);

                if (exists)
                    return Ok(ApiResult<DynamicTableDto>.Failure("کد در این جدول تکراری است", 400));

                var table = new DynamicTable
                {
                    TableName = model.TableName,
                    Code = model.Code,
                    Value = model.Value,
                    ParentId = model.ParentId,
                    DisplayOrder = model.DisplayOrder,
                    IsActive = model.IsActive,
                    CreatedBy = userId,
                    CreatedAt = DateTime.Now
                };

                _context.DynamicTables.Add(table);
                await _context.SaveChangesAsync();

                var dto = new DynamicTableDto
                {
                    Id = table.Id,
                    TableName = table.TableName,
                    Code = table.Code,
                    Value = table.Value,
                    ParentId = table.ParentId,
                    DisplayOrder = table.DisplayOrder,
                    IsActive = table.IsActive
                };

                Log.Information($"رکورد جدید در جدول {model.TableName} ایجاد شد: {model.Code}");
                return Ok(ApiResult<DynamicTableDto>.Success(dto, "رکورد با موفقیت ایجاد شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "خطا در ایجاد رکورد جدول پویا");
                return Ok(ApiResult<DynamicTableDto>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// ویرایش رکورد جدول پویا
        /// PUT: api/DynamicTables/{id}
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ApiResult<DynamicTableDto>>> Update(int id, [FromBody] CreateDynamicTableDto model)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                var table = await _context.DynamicTables.FindAsync(id);
                if (table == null || table.IsDeleted)
                    return Ok(ApiResult<DynamicTableDto>.Failure("رکورد یافت نشد", 404));

                // بررسی تکراری بودن (به جز خودش)
                var exists = await _context.DynamicTables
                    .AnyAsync(t => t.Id != id && t.TableName == model.TableName && t.Code == model.Code && !t.IsDeleted);

                if (exists)
                    return Ok(ApiResult<DynamicTableDto>.Failure("کد در این جدول تکراری است", 400));

                table.TableName = model.TableName;
                table.Code = model.Code;
                table.Value = model.Value;
                table.ParentId = model.ParentId;
                table.DisplayOrder = model.DisplayOrder;
                table.IsActive = model.IsActive;
                table.UpdatedBy = userId;
                table.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                var dto = new DynamicTableDto
                {
                    Id = table.Id,
                    TableName = table.TableName,
                    Code = table.Code,
                    Value = table.Value,
                    ParentId = table.ParentId,
                    DisplayOrder = table.DisplayOrder,
                    IsActive = table.IsActive
                };

                Log.Information($"رکورد جدول {table.TableName} ویرایش شد: {table.Code}");
                return Ok(ApiResult<DynamicTableDto>.Success(dto, "رکورد با موفقیت ویرایش شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در ویرایش رکورد {id}");
                return Ok(ApiResult<DynamicTableDto>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// حذف رکورد جدول پویا
        /// DELETE: api/DynamicTables/{id}
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResult>> Delete(int id)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                var table = await _context.DynamicTables.FindAsync(id);
                if (table == null || table.IsDeleted)
                    return Ok(ApiResult.Failure("رکورد یافت نشد", 404));

                table.IsDeleted = true;
                table.UpdatedBy = userId;
                table.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                Log.Information($"رکورد جدول {table.TableName} حذف شد: {table.Code}");
                return Ok(ApiResult.Success("رکورد با موفقیت حذف شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در حذف رکورد {id}");
                return Ok(ApiResult.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// فعال/غیرفعال کردن رکورد
        /// PATCH: api/DynamicTables/{id}/Toggle
        /// </summary>
        [HttpPatch("{id}/Toggle")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ApiResult>> ToggleActive(int id)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                var table = await _context.DynamicTables.FindAsync(id);
                if (table == null || table.IsDeleted)
                    return Ok(ApiResult.Failure("رکورد یافت نشد", 404));

                table.IsActive = !table.IsActive;
                table.UpdatedBy = userId;
                table.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                var status = table.IsActive ? "فعال" : "غیرفعال";
                Log.Information($"رکورد جدول {table.TableName} {status} شد: {table.Code}");

                return Ok(ApiResult.Success($"رکورد {status} شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در تغییر وضعیت رکورد {id}");
                return Ok(ApiResult.Failure("خطای سیستمی رخ داده است", 500));
            }
        }
    }
}
