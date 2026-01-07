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
    /// کنترلر مدیریت واحدهای سازمانی
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrganizationUnitsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrganizationUnitsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// دریافت تمام واحدهای سازمانی فعال
        /// GET: api/OrganizationUnits
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResult<List<OrganizationUnitDto>>>> GetAll()
        {
            try
            {
                var units = await _context.OrganizationUnits
                    .Where(u => !u.IsDeleted)
                    .OrderBy(u => u.Name)
                    .Select(u => new OrganizationUnitDto
                    {
                        Id = u.Id,
                        Name = u.Name,
                        UnitType = u.UnitType,
                        Code = u.Code,
                        ParentUnitId = u.ParentUnitId,
                        ProvinceId = u.ProvinceId,
                        RegionId = u.RegionId,
                        SchoolId = u.SchoolId,
                        Address = u.Address,
                        PhoneNumber = u.PhoneNumber,
                        IsActive = u.IsActive
                    })
                    .ToListAsync();

                return Ok(ApiResult<List<OrganizationUnitDto>>.Success(units, $"{units.Count} واحد یافت شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "خطا در دریافت واحدهای سازمانی");
                return Ok(ApiResult<List<OrganizationUnitDto>>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// دریافت یک واحد سازمانی
        /// GET: api/OrganizationUnits/{id}
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<OrganizationUnitDto>>> GetById(int id)
        {
            try
            {
                var unit = await _context.OrganizationUnits
                    .Where(u => u.Id == id && !u.IsDeleted)
                    .Select(u => new OrganizationUnitDto
                    {
                        Id = u.Id,
                        Name = u.Name,
                        UnitType = u.UnitType,
                        Code = u.Code,
                        ParentUnitId = u.ParentUnitId,
                        ProvinceId = u.ProvinceId,
                        RegionId = u.RegionId,
                        SchoolId = u.SchoolId,
                        Address = u.Address,
                        PhoneNumber = u.PhoneNumber,
                        IsActive = u.IsActive
                    })
                    .FirstOrDefaultAsync();

                if (unit == null)
                    return Ok(ApiResult<OrganizationUnitDto>.Failure("واحد سازمانی یافت نشد", 404));

                return Ok(ApiResult<OrganizationUnitDto>.Success(unit));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در دریافت واحد سازمانی {id}");
                return Ok(ApiResult<OrganizationUnitDto>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// دریافت زیرمجموعه‌های یک واحد
        /// GET: api/OrganizationUnits/Children/{parentId}
        /// </summary>
        [HttpGet("Children/{parentId}")]
        public async Task<ActionResult<ApiResult<List<OrganizationUnitDto>>>> GetChildren(int parentId)
        {
            try
            {
                var children = await _context.OrganizationUnits
                    .Where(u => u.ParentUnitId == parentId && !u.IsDeleted)
                    .OrderBy(u => u.Name)
                    .Select(u => new OrganizationUnitDto
                    {
                        Id = u.Id,
                        Name = u.Name,
                        UnitType = u.UnitType,
                        Code = u.Code,
                        ParentUnitId = u.ParentUnitId,
                        ProvinceId = u.ProvinceId,
                        RegionId = u.RegionId,
                        SchoolId = u.SchoolId,
                        Address = u.Address,
                        PhoneNumber = u.PhoneNumber,
                        IsActive = u.IsActive
                    })
                    .ToListAsync();

                return Ok(ApiResult<List<OrganizationUnitDto>>.Success(children, $"{children.Count} واحد فرعی یافت شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در دریافت واحدهای فرعی {parentId}");
                return Ok(ApiResult<List<OrganizationUnitDto>>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// ایجاد واحد سازمانی جدید
        /// POST: api/OrganizationUnits
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ApiResult<OrganizationUnitDto>>> Create([FromBody] CreateOrganizationUnitDto model)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                // بررسی تکراری بودن نام
                var exists = await _context.OrganizationUnits
                    .AnyAsync(u => u.Name == model.Name && !u.IsDeleted);
                if (exists)
                    return Ok(ApiResult<OrganizationUnitDto>.Failure("نام واحد سازمانی تکراری است", 400));

                var unit = new OrganizationUnit
                {
                    Name = model.Name,
                    UnitType = model.UnitType,
                    Code = model.Code,
                    ParentUnitId = model.ParentUnitId,
                    ProvinceId = model.ProvinceId,
                    RegionId = model.RegionId,
                    SchoolId = model.SchoolId,
                    IsActive = true,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber,
                    CreatedBy = userId,
                    CreatedAt = DateTime.Now
                };

                _context.OrganizationUnits.Add(unit);
                await _context.SaveChangesAsync();

                var dto = new OrganizationUnitDto
                {
                    Id = unit.Id,
                    Name = unit.Name,
                    UnitType = unit.UnitType,
                    Code = unit.Code,
                    ParentUnitId = unit.ParentUnitId,
                    ProvinceId = unit.ProvinceId,
                    RegionId = unit.RegionId,
                    SchoolId = unit.SchoolId,
                    Address = unit.Address,
                    PhoneNumber = unit.PhoneNumber,
                    IsActive = unit.IsActive
                };

                Log.Information($"واحد سازمانی جدید ایجاد شد: {unit.Name}");
                return Ok(ApiResult<OrganizationUnitDto>.Success(dto, "واحد سازمانی با موفقیت ایجاد شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "خطا در ایجاد واحد سازمانی");
                return Ok(ApiResult<OrganizationUnitDto>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// ویرایش واحد سازمانی
        /// PUT: api/OrganizationUnits/{id}
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ApiResult<OrganizationUnitDto>>> Update(int id, [FromBody] CreateOrganizationUnitDto model)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                var unit = await _context.OrganizationUnits.FindAsync(id);
                if (unit == null || unit.IsDeleted)
                    return Ok(ApiResult<OrganizationUnitDto>.Failure("واحد سازمانی یافت نشد", 404));

                // بررسی تکراری بودن نام
                var exists = await _context.OrganizationUnits
                    .AnyAsync(u => u.Name == model.Name && u.Id != id && !u.IsDeleted);
                if (exists)
                    return Ok(ApiResult<OrganizationUnitDto>.Failure("نام واحد سازمانی تکراری است", 400));

                unit.Name = model.Name;
                unit.UnitType = model.UnitType;
                unit.Code = model.Code;
                unit.ParentUnitId = model.ParentUnitId;
                unit.ProvinceId = model.ProvinceId;
                unit.RegionId = model.RegionId;
                unit.SchoolId = model.SchoolId;
                unit.Address = model.Address;
                unit.PhoneNumber = model.PhoneNumber;
                unit.UpdatedBy = userId;
                unit.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                var dto = new OrganizationUnitDto
                {
                    Id = unit.Id,
                    Name = unit.Name,
                    UnitType = unit.UnitType,
                    Code = unit.Code,
                    ParentUnitId = unit.ParentUnitId,
                    ProvinceId = unit.ProvinceId,
                    RegionId = unit.RegionId,
                    SchoolId = unit.SchoolId,
                    Address = unit.Address,
                    PhoneNumber = unit.PhoneNumber,
                    IsActive = unit.IsActive
                };

                Log.Information($"واحد سازمانی {id} ویرایش شد");
                return Ok(ApiResult<OrganizationUnitDto>.Success(dto, "واحد سازمانی با موفقیت ویرایش شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در ویرایش واحد سازمانی {id}");
                return Ok(ApiResult<OrganizationUnitDto>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// حذف واحد سازمانی
        /// DELETE: api/OrganizationUnits/{id}
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResult>> Delete(int id)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                var unit = await _context.OrganizationUnits.FindAsync(id);
                if (unit == null || unit.IsDeleted)
                    return Ok(ApiResult.Failure("واحد سازمانی یافت نشد", 404));

                // بررسی داشتن زیرمجموعه
                var hasChildren = await _context.OrganizationUnits
                    .AnyAsync(u => u.ParentUnitId == id && !u.IsDeleted);
                if (hasChildren)
                    return Ok(ApiResult.Failure("واحد سازمانی دارای زیرمجموعه است", 400));

                unit.IsDeleted = true;
                unit.UpdatedBy = userId;
                unit.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                Log.Information($"واحد سازمانی {id} حذف شد");
                return Ok(ApiResult.Success("واحد سازمانی با موفقیت حذف شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در حذف واحد سازمانی {id}");
                return Ok(ApiResult.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// تغییر وضعیت فعال/غیرفعال
        /// PATCH: api/OrganizationUnits/{id}/Toggle
        /// </summary>
        [HttpPatch("{id}/Toggle")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ApiResult>> ToggleActive(int id)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                var unit = await _context.OrganizationUnits.FindAsync(id);
                if (unit == null || unit.IsDeleted)
                    return Ok(ApiResult.Failure("واحد سازمانی یافت نشد", 404));

                unit.IsActive = !unit.IsActive;
                unit.UpdatedBy = userId;
                unit.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                Log.Information($"وضعیت واحد سازمانی {id} تغییر کرد: {unit.IsActive}");
                return Ok(ApiResult.Success($"وضعیت واحد به {(unit.IsActive ? "فعال" : "غیرفعال")} تغییر یافت"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در تغییر وضعیت واحد سازمانی {id}");
                return Ok(ApiResult.Failure("خطای سیستمی رخ داده است", 500));
            }
        }
    }
}
