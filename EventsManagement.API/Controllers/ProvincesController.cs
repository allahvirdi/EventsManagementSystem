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
    /// کنترلر مدیریت استان‌ها
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProvincesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProvincesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// دریافت تمام استان‌ها
        /// GET: api/Provinces
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResult<List<ProvinceDto>>>> GetAll()
        {
            try
            {
                var provinces = await _context.Provinces
                    .Where(p => !p.IsDeleted)
                    .OrderBy(p => p.Name)
                    .Select(p => new ProvinceDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Code = p.Code
                    })
                    .ToListAsync();

                return Ok(ApiResult<List<ProvinceDto>>.Success(provinces, $"{provinces.Count} استان یافت شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "خطا در دریافت استان‌ها");
                return Ok(ApiResult<List<ProvinceDto>>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// دریافت یک استان
        /// GET: api/Provinces/{id}
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<ProvinceDto>>> GetById(int id)
        {
            try
            {
                var province = await _context.Provinces
                    .Where(p => p.Id == id && !p.IsDeleted)
                    .Select(p => new ProvinceDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Code = p.Code
                    })
                    .FirstOrDefaultAsync();

                if (province == null)
                    return Ok(ApiResult<ProvinceDto>.Failure("استان یافت نشد", 404));

                return Ok(ApiResult<ProvinceDto>.Success(province));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در دریافت استان {id}");
                return Ok(ApiResult<ProvinceDto>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// ایجاد استان جدید
        /// POST: api/Provinces
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResult<ProvinceDto>>> Create([FromBody] CreateProvinceDto model)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                // بررسی تکراری بودن نام
                var exists = await _context.Provinces
                    .AnyAsync(p => p.Name == model.Name && !p.IsDeleted);
                if (exists)
                    return Ok(ApiResult<ProvinceDto>.Failure("نام استان تکراری است", 400));

                // بررسی تکراری بودن کد
                var codeExists = await _context.Provinces
                    .AnyAsync(p => p.Code == model.Code && !p.IsDeleted);
                if (codeExists)
                    return Ok(ApiResult<ProvinceDto>.Failure("کد استان تکراری است", 400));

                var province = new Province
                {
                    Name = model.Name,
                    Code = model.Code,
                    CreatedBy = userId,
                    CreatedAt = DateTime.Now
                };

                _context.Provinces.Add(province);
                await _context.SaveChangesAsync();

                var dto = new ProvinceDto
                {
                    Id = province.Id,
                    Name = province.Name,
                    Code = province.Code
                };

                Log.Information($"استان جدید ایجاد شد: {province.Name}");
                return Ok(ApiResult<ProvinceDto>.Success(dto, "استان با موفقیت ایجاد شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "خطا در ایجاد استان");
                return Ok(ApiResult<ProvinceDto>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// ویرایش استان
        /// PUT: api/Provinces/{id}
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResult<ProvinceDto>>> Update(int id, [FromBody] CreateProvinceDto model)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                var province = await _context.Provinces.FindAsync(id);
                if (province == null || province.IsDeleted)
                    return Ok(ApiResult<ProvinceDto>.Failure("استان یافت نشد", 404));

                // بررسی تکراری بودن نام
                var exists = await _context.Provinces
                    .AnyAsync(p => p.Name == model.Name && p.Id != id && !p.IsDeleted);
                if (exists)
                    return Ok(ApiResult<ProvinceDto>.Failure("نام استان تکراری است", 400));

                // بررسی تکراری بودن کد
                var codeExists = await _context.Provinces
                    .AnyAsync(p => p.Code == model.Code && p.Id != id && !p.IsDeleted);
                if (codeExists)
                    return Ok(ApiResult<ProvinceDto>.Failure("کد استان تکراری است", 400));

                province.Name = model.Name;
                province.Code = model.Code;
                province.UpdatedBy = userId;
                province.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                var dto = new ProvinceDto
                {
                    Id = province.Id,
                    Name = province.Name,
                    Code = province.Code
                };

                Log.Information($"استان {id} ویرایش شد");
                return Ok(ApiResult<ProvinceDto>.Success(dto, "استان با موفقیت ویرایش شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در ویرایش استان {id}");
                return Ok(ApiResult<ProvinceDto>.Failure("خطای سیستمی رخ داده است", 500));
            }
        }

        /// <summary>
        /// حذف استان
        /// DELETE: api/Provinces/{id}
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResult>> Delete(int id)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                var province = await _context.Provinces.FindAsync(id);
                if (province == null || province.IsDeleted)
                    return Ok(ApiResult.Failure("استان یافت نشد", 404));

                // بررسی وابستگی
                var hasRegions = await _context.Regions
                    .AnyAsync(r => r.ProvinceId == id && !r.IsDeleted);
                if (hasRegions)
                    return Ok(ApiResult.Failure("استان دارای مناطق وابسته است", 400));

                province.IsDeleted = true;
                province.UpdatedBy = userId;
                province.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                Log.Information($"استان {id} حذف شد");
                return Ok(ApiResult.Success("استان با موفقیت حذف شد"));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"خطا در حذف استان {id}");
                return Ok(ApiResult.Failure("خطای سیستمی رخ داده است", 500));
            }
        }
    }
}
